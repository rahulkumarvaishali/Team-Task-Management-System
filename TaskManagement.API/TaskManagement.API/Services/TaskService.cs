using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Tasks;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Models;
using TaskStatusEnum = TaskManagement.API.Models.Enums.TaskStatus;

namespace TaskManagement.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public TaskService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;

        }

        public async Task<IEnumerable<TaskDto>> GetTasksAsync(
    int currentUserId,
    string currentUserRole,
    int? status = null,
    int? priority = null,
    DateTime? deadline = null)
        {
            // Start with all tasks
            var query = _context.Tasks
                .AsNoTracking()
                .AsQueryable();

            // =========================
            // ROLE-BASED FILTERING
            // =========================

            // Normal User can see only tasks assigned to them
            if (currentUserRole == "User")
            {
                query = query.Where(t =>
                    t.AssignedToUserId == currentUserId);
            }

            // Manager can see only tasks belonging
            // to teams managed by them
            else if (currentUserRole == "Manager")
            {
                var managerTeamIds = _context.Teams
                    .Where(t => t.ManagerId == currentUserId)
                    .Select(t => t.Id);

                query = query.Where(t =>
                    t.TeamId.HasValue &&
                    managerTeamIds.Contains(t.TeamId.Value));
            }

            // Admin gets all tasks
            // No additional filtering required


            // =========================
            // STATUS FILTER
            // =========================

            if (status.HasValue)
            {
                query = query.Where(t =>
                    (int)t.Status == status.Value);
            }


            // =========================
            // PRIORITY FILTER
            // =========================

            if (priority.HasValue)
            {
                query = query.Where(t =>
                    (int)t.Priority == priority.Value);
            }


            // =========================
            // DEADLINE FILTER
            // =========================

            if (deadline.HasValue)
            {
                var startDate = deadline.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(t =>
                    t.Deadline.HasValue &&
                    t.Deadline.Value >= startDate &&
                    t.Deadline.Value < endDate);
            }


            // =========================
            // RETURN DTO
            // =========================

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TaskDto
                {
                    Id = t.Id,

                    Title = t.Title,

                    Description = t.Description,

                    Status = t.Status.ToString(),

                    Priority = t.Priority.ToString(),

                    Deadline = t.Deadline,

                    AssignedToUserId =
                        t.AssignedToUserId,

                    AssignedToUserName =
                        t.AssignedToUser != null
                            ? t.AssignedToUser.FullName
                            : null,

                    CreatedByUserId =
                        t.CreatedByUserId,

                    CreatedByUserName =
                        t.CreatedByUser.FullName,

                    TeamId =
                        t.TeamId,

                    TeamName =
                        t.Team != null
                            ? t.Team.Name
                            : null,

                    CreatedAt =
                        t.CreatedAt,

                    UpdatedAt =
                        t.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(
            int taskId,
            int currentUserId,
            string currentUserRole)
        {
            var tasks = await GetTasksAsync(
                currentUserId,
                currentUserRole);

            return tasks.FirstOrDefault(t =>
                t.Id == taskId);
        }

        public async Task<TaskDto?> CreateTaskAsync(
    CreateTaskDto dto,
    int currentUserId,
    string currentUserRole)
        {
            // Normal users cannot create tasks
            if (currentUserRole == "User")
            {
                return null;
            }

            // =====================================
            // MANAGER VALIDATION
            // =====================================

            if (currentUserRole == "Manager")
            {
                if (!dto.TeamId.HasValue)
                {
                    return null;
                }

                var managerOwnsTeam =
                    await _context.Teams.AnyAsync(t =>
                        t.Id == dto.TeamId.Value &&
                        t.ManagerId == currentUserId);

                if (!managerOwnsTeam)
                {
                    return null;
                }
            }

            // =====================================
            // TEAM VALIDATION
            // =====================================

            if (dto.TeamId.HasValue)
            {
                var teamExists =
                    await _context.Teams.AnyAsync(t =>
                        t.Id == dto.TeamId.Value);

                if (!teamExists)
                {
                    return null;
                }
            }

            // =====================================
            // ASSIGNED USER VALIDATION
            // =====================================

            if (dto.AssignedToUserId.HasValue)
            {
                var assignedUser =
                    await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u =>
                            u.Id == dto.AssignedToUserId.Value);

                if (assignedUser == null)
                {
                    return null;
                }

                if (dto.TeamId.HasValue &&
                    assignedUser.TeamId != dto.TeamId.Value)
                {
                    return null;
                }

                if (currentUserRole == "Manager")
                {
                    if (!dto.TeamId.HasValue ||
                        assignedUser.TeamId != dto.TeamId.Value)
                    {
                        return null;
                    }
                }
            }

            // =====================================
            // CREATE TASK
            // =====================================

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description,
                Priority = dto.Priority,

                Status = TaskStatusEnum.ToDo,

                Deadline = dto.Deadline,

                AssignedToUserId =
                    dto.AssignedToUserId,

                TeamId =
                    dto.TeamId,

                CreatedByUserId =
                    currentUserId,

                CreatedAt =
                    DateTime.UtcNow
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            // =====================================
            // ACTIVITY - TASK CREATED
            // =====================================

            await AddActivityAsync(
                task.Id,
                currentUserId,
                currentUserRole,
                "TaskCreated",
                "Created the task."
            );

            // =====================================
            // ACTIVITY - INITIAL ASSIGNMENT
            // =====================================

            if (task.AssignedToUserId.HasValue)
            {
                var assignedUser =
                    await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u =>
                            u.Id == task.AssignedToUserId.Value);

                if (assignedUser != null)
                {
                    await AddActivityAsync(
                        task.Id,
                        currentUserId,
                        currentUserRole,
                        "TaskAssigned",
                        $"Assigned task to {assignedUser.FullName}."
                    );
                }
            }

            // =====================================
            // ASSIGNMENT NOTIFICATION
            // =====================================

            if (task.AssignedToUserId.HasValue)
            {
                await _notificationService
                    .CreateNotificationAsync(
                        task.AssignedToUserId.Value,
                        $"A new task '{task.Title}' has been assigned to you.",
                        "TaskAssigned");
            }

            return await GetTaskForResponseAsync(task.Id);
        }

        public async Task<bool> UpdateTaskAsync(
    int taskId,
    UpdateTaskDto dto,
    int currentUserId,
    string currentUserRole)
        {
            // Normal User cannot update full task details
            if (currentUserRole == "User")
            {
                return false;
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return false;
            }

            // =====================================
            // DO NOT EDIT COMPLETED TASK
            // =====================================

            if (task.Status == TaskStatusEnum.Done)
            {
                return false;
            }

            // =====================================
            // MANAGER - CURRENT TASK VALIDATION
            // =====================================

            if (currentUserRole == "Manager")
            {
                if (!task.TeamId.HasValue)
                {
                    return false;
                }

                var ownsCurrentTeam =
                    await _context.Teams.AnyAsync(t =>
                        t.Id == task.TeamId.Value &&
                        t.ManagerId == currentUserId);

                if (!ownsCurrentTeam)
                {
                    return false;
                }

                if (!dto.TeamId.HasValue)
                {
                    return false;
                }
            }

            // =====================================
            // NEW TEAM VALIDATION
            // =====================================

            if (dto.TeamId.HasValue)
            {
                var newTeam =
                    await _context.Teams
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t =>
                            t.Id == dto.TeamId.Value);

                if (newTeam == null)
                {
                    return false;
                }

                if (currentUserRole == "Manager" &&
                    newTeam.ManagerId != currentUserId)
                {
                    return false;
                }
            }

            // =====================================
            // ASSIGNED USER VALIDATION
            // =====================================

            if (dto.AssignedToUserId.HasValue)
            {
                var assignedUser =
                    await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u =>
                            u.Id == dto.AssignedToUserId.Value);

                if (assignedUser == null)
                {
                    return false;
                }

                if (dto.TeamId.HasValue &&
                    assignedUser.TeamId != dto.TeamId.Value)
                {
                    return false;
                }

                if (currentUserRole == "Manager")
                {
                    if (!dto.TeamId.HasValue ||
                        assignedUser.TeamId != dto.TeamId.Value)
                    {
                        return false;
                    }
                }
            }

            // =====================================
            // SAVE OLD VALUES FOR HISTORY
            // =====================================

            var oldAssignedUserId =
                task.AssignedToUserId;

            var oldPriority =
                task.Priority;

            var oldDeadline =
                task.Deadline;

            // =====================================
            // UPDATE TASK
            // =====================================

            task.Title =
                dto.Title.Trim();

            task.Description =
                dto.Description;

            task.Priority =
                dto.Priority;

            task.Deadline =
                dto.Deadline;

            task.AssignedToUserId =
                dto.AssignedToUserId;

            task.TeamId =
                dto.TeamId;

            task.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // =====================================
            // ACTIVITY - ASSIGNMENT CHANGED
            // =====================================

            if (oldAssignedUserId != task.AssignedToUserId)
            {
                string assignedUserName = "Unassigned";

                if (task.AssignedToUserId.HasValue)
                {
                    var assignedUser =
                        await _context.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u =>
                                u.Id ==
                                task.AssignedToUserId.Value);

                    if (assignedUser != null)
                    {
                        assignedUserName =
                            assignedUser.FullName;
                    }
                }

                await AddActivityAsync(
                    task.Id,
                    currentUserId,
                    currentUserRole,
                    "TaskAssigned",
                    $"Task assigned to {assignedUserName}."
                );
            }

            // =====================================
            // ACTIVITY - PRIORITY CHANGED
            // =====================================

            if (oldPriority != task.Priority)
            {
                await AddActivityAsync(
                    task.Id,
                    currentUserId,
                    currentUserRole,
                    "PriorityChanged",
                    $"Priority changed from {oldPriority} to {task.Priority}."
                );
            }

            // =====================================
            // ACTIVITY - DEADLINE CHANGED
            // =====================================

            if (oldDeadline != task.Deadline)
            {
                var oldDate =
                    oldDeadline?.ToString("dd MMM yyyy")
                    ?? "No deadline";

                var newDate =
                    task.Deadline?.ToString("dd MMM yyyy")
                    ?? "No deadline";

                await AddActivityAsync(
                    task.Id,
                    currentUserId,
                    currentUserRole,
                    "DeadlineChanged",
                    $"Deadline changed from {oldDate} to {newDate}."
                );
            }

            // =====================================
            // REASSIGNMENT NOTIFICATION
            // =====================================

            if (dto.AssignedToUserId.HasValue &&
                oldAssignedUserId != dto.AssignedToUserId)
            {
                await _notificationService
                    .CreateNotificationAsync(
                        dto.AssignedToUserId.Value,
                        $"Task '{task.Title}' has been assigned to you.",
                        "TaskAssigned");
            }

            return true;
        }

        public async Task<bool> UpdateTaskStatusAsync(
    int taskId,
    UpdateTaskStatusDto dto,
    int currentUserId,
    string currentUserRole)
        {
            var task =
                await _context.Tasks
                    .FirstOrDefaultAsync(t =>
                        t.Id == taskId);

            if (task == null)
            {
                return false;
            }

            // =====================================
            // USER ACCESS
            // =====================================

            if (currentUserRole == "User" &&
                task.AssignedToUserId != currentUserId)
            {
                return false;
            }

            // =====================================
            // MANAGER ACCESS
            // =====================================

            if (currentUserRole == "Manager")
            {
                var ownsTeam =
                    await _context.Teams.AnyAsync(t =>
                        t.Id == task.TeamId &&
                        t.ManagerId == currentUserId);

                if (!ownsTeam)
                {
                    return false;
                }
            }

            // =====================================
            // COMPLETED TASK CANNOT GO BACK
            // =====================================

            if (task.Status == TaskStatusEnum.Done &&
                dto.Status != TaskStatusEnum.Done)
            {
                return false;
            }

            // No actual change
            if (task.Status == dto.Status)
            {
                return true;
            }

            // =====================================
            // SAVE OLD STATUS
            // =====================================

            var oldStatus =
                task.Status;

            // =====================================
            // UPDATE STATUS
            // =====================================

            task.Status =
                dto.Status;

            task.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // =====================================
            // ACTIVITY - STATUS CHANGED
            // =====================================

            await AddActivityAsync(
                task.Id,
                currentUserId,
                currentUserRole,
                "StatusChanged",
                $"Status changed from {oldStatus} to {task.Status}."
            );

            // =====================================
            // NOTIFICATION
            // =====================================

            if (task.CreatedByUserId != currentUserId)
            {
                await _notificationService
                    .CreateNotificationAsync(
                        task.CreatedByUserId,
                        $"Task '{task.Title}' status changed to {task.Status}.",
                        "TaskStatusUpdated");
            }

            return true;
        }

        public async Task<bool> DeleteTaskAsync(
            int taskId,
            int currentUserId,
            string currentUserRole)
        {
            // Normal user cannot delete
            if (currentUserRole == "User")
            {
                return false;
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t =>
                    t.Id == taskId);

            if (task == null)
            {
                return false;
            }

            if (currentUserRole == "Manager")
            {
                var ownsTeam =
                    await _context.Teams.AnyAsync(t =>
                        t.Id == task.TeamId &&
                        t.ManagerId == currentUserId);

                if (!ownsTeam)
                {
                    return false;
                }
            }

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<TaskDto?> GetTaskForResponseAsync(
            int taskId)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(t => t.Id == taskId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,

                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),

                    Deadline = t.Deadline,

                    AssignedToUserId =
                        t.AssignedToUserId,

                    AssignedToUserName =
                        t.AssignedToUser != null
                            ? t.AssignedToUser.FullName
                            : null,

                    CreatedByUserId =
                        t.CreatedByUserId,

                    CreatedByUserName =
                        t.CreatedByUser.FullName,

                    TeamId = t.TeamId,

                    TeamName =
                        t.Team != null
                            ? t.Team.Name
                            : null,

                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DashboardDto> GetDashboardAsync(
    int currentUserId,
    string currentUserRole)
        {
            var query = _context.Tasks
                .AsNoTracking()
                .AsQueryable();

            // Normal User:
            // only their assigned tasks
            if (currentUserRole == "User")
            {
                query = query.Where(t =>
                    t.AssignedToUserId == currentUserId);
            }

            // Manager:
            // only tasks belonging to teams they manage
            else if (currentUserRole == "Manager")
            {
                var teamIds = _context.Teams
                    .Where(t => t.ManagerId == currentUserId)
                    .Select(t => t.Id);

                query = query.Where(t =>
                    t.TeamId.HasValue &&
                    teamIds.Contains(t.TeamId.Value));
            }

            // Admin:
            // no filter -> sees all tasks

            var now = DateTime.UtcNow;

            return new DashboardDto
            {
                TotalTasks =
                    await query.CountAsync(),

                ToDoTasks =
                    await query.CountAsync(t =>
                        t.Status ==
                        TaskManagement.API.Models.Enums.TaskStatus.ToDo),

                InProgressTasks =
                    await query.CountAsync(t =>
                        t.Status ==
                        TaskManagement.API.Models.Enums.TaskStatus.InProgress),

                DoneTasks =
                    await query.CountAsync(t =>
                        t.Status ==
                        TaskManagement.API.Models.Enums.TaskStatus.Done),

                LowPriorityTasks =
                    await query.CountAsync(t =>
                        t.Priority ==
                        TaskManagement.API.Models.Enums.TaskPriority.Low),

                MediumPriorityTasks =
                    await query.CountAsync(t =>
                        t.Priority ==
                        TaskManagement.API.Models.Enums.TaskPriority.Medium),

                HighPriorityTasks =
                    await query.CountAsync(t =>
                        t.Priority ==
                        TaskManagement.API.Models.Enums.TaskPriority.High),

                OverdueTasks =
                    await query.CountAsync(t =>
                        t.Deadline.HasValue &&
                        t.Deadline.Value < now &&
                        t.Status !=
                        TaskManagement.API.Models.Enums.TaskStatus.Done)
            };
        }

        private async Task AddActivityAsync(
    int taskId,
    int performedByUserId,
    string role,
    string action,
    string details)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Id == performedByUserId);

            var activity = new TaskActivity
            {
                TaskId = taskId,

                PerformedByUserId =
                    performedByUserId,

                PerformedByName =
                    user?.FullName ?? "Unknown User",

                PerformedByRole = role,

                Action = action,

                Details = details,

                CreatedAt = DateTime.UtcNow
            };

            _context.TaskActivities.Add(activity);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskActivityDto>>
    GetTaskActivityAsync(int taskId)
        {
            return await _context.TaskActivities
                .AsNoTracking()
                .Where(a => a.TaskId == taskId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new TaskActivityDto
                {
                    Id = a.Id,

                    PerformedByName =
                        a.PerformedByName,

                    PerformedByRole =
                        a.PerformedByRole,

                    Action =
                        a.Action,

                    Details =
                        a.Details,

                    CreatedAt =
                        a.CreatedAt
                })
                .ToListAsync();
        }
    }
}
