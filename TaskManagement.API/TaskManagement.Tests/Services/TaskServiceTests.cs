using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Tasks;
using TaskManagement.API.Models;
using TaskManagement.API.Models.Enums;
using TaskManagement.API.Services;

using TaskManagement.Tests.Helpers;

using TaskStatusEnum =
    TaskManagement.API.Models.Enums.TaskStatus;

namespace TaskManagement.Tests.Services
{
    public class TaskServiceTests
    {
        // ==========================================
        // CREATE TEST DATABASE
        // ==========================================

        private ApplicationDbContext CreateContext()
        {
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(
                        Guid.NewGuid().ToString())
                    .Options;

            return new ApplicationDbContext(options);
        }

        // ==========================================
        // CREATE SERVICE
        // ==========================================

        private TaskService CreateService(
            ApplicationDbContext context)
        {
            var notificationService =
                new FakeNotificationService();

            return new TaskService(
                context,
                notificationService);
        }

        // ==========================================
        // TEST 1
        // USER CANNOT CREATE TASK
        // ==========================================

        [Fact]
        public async Task CreateTask_AsUser_ReturnsNull()
        {
            // Arrange

            using var context =
                CreateContext();

            var service =
                CreateService(context);

            var dto =
                new CreateTaskDto
                {
                    Title = "Test Task",
                    Description =
                        "Test Description",

                    Priority =
                        TaskPriority.Medium,

                    Deadline =
                        DateTime.UtcNow.AddDays(2)
                };

            // Act

            var result =
                await service.CreateTaskAsync(
                    dto,
                    1,
                    "User");

            // Assert

            Assert.Null(result);

            Assert.Empty(
                await context.Tasks.ToListAsync());
        }

        // ==========================================
        // TEST 2
        // ADMIN CAN CREATE TASK
        // ==========================================

        [Fact]
        public async Task CreateTask_AsAdmin_CreatesTask()
        {
            // Arrange

            using var context =
                CreateContext();

            var admin =
                new User
                {
                    Id = 1,
                    FullName = "Test Admin",
                    Email =
                        "admin@test.com",

                    PasswordHash =
                        "test-password-hash",

                    Role =
                        UserRole.Admin
                };

            context.Users.Add(admin);

            await context.SaveChangesAsync();

            var service =
                CreateService(context);

            var dto =
                new CreateTaskDto
                {
                    Title =
                        "Admin Test Task",

                    Description =
                        "Created during unit test",

                    Priority =
                        TaskPriority.High,

                    Deadline =
                        DateTime.UtcNow.AddDays(3)
                };

            // Act

            var result =
                await service.CreateTaskAsync(
                    dto,
                    admin.Id,
                    "Admin");

            // Assert

            Assert.NotNull(result);

            Assert.Equal(
                "Admin Test Task",
                result.Title);

            Assert.Equal(
                "ToDo",
                result.Status);

            Assert.Single(
                await context.Tasks.ToListAsync());

            // Activity history should
            // also have been created.

            var activity =
                await context.TaskActivities
                    .FirstOrDefaultAsync();

            Assert.NotNull(activity);

            Assert.Equal(
                "TaskCreated",
                activity.Action);

            Assert.Equal(
                "Test Admin",
                activity.PerformedByName);

            Assert.Equal(
                "Admin",
                activity.PerformedByRole);
        }

        // ==========================================
        // TEST 3
        // MANAGER CAN CREATE TASK FOR OWN TEAM
        // ==========================================

        [Fact]
        public async Task
            CreateTask_AsManagerForOwnTeam_CreatesTask()
        {
            // Arrange

            using var context =
                CreateContext();

            var manager =
                new User
                {
                    Id = 1,
                    FullName =
                        "Test Manager",

                    Email =
                        "manager@test.com",

                    PasswordHash =
                        "test-password-hash",

                    Role =
                        UserRole.Manager
                };

            context.Users.Add(manager);

            await context.SaveChangesAsync();

            var team =
                new Team
                {
                    Id = 1,
                    Name =
                        "Development Team",

                    Description =
                        "Test Team",

                    ManagerId =
                        manager.Id,

                    CreatedAt =
                        DateTime.UtcNow
                };

            context.Teams.Add(team);

            await context.SaveChangesAsync();

            var service =
                CreateService(context);

            var dto =
                new CreateTaskDto
                {
                    Title =
                        "Manager Task",

                    Description =
                        "Manager test task",

                    Priority =
                        TaskPriority.Medium,

                    Deadline =
                        DateTime.UtcNow.AddDays(2),

                    TeamId =
                        team.Id
                };

            // Act

            var result =
                await service.CreateTaskAsync(
                    dto,
                    manager.Id,
                    "Manager");

            // Assert

            Assert.NotNull(result);

            Assert.Equal(
                team.Id,
                result.TeamId);

            Assert.Equal(
                "Manager Task",
                result.Title);
        }

        // ==========================================
        // TEST 4
        // MANAGER CANNOT CREATE TASK
        // FOR ANOTHER MANAGER'S TEAM
        // ==========================================

        [Fact]
        public async Task
            CreateTask_AsManagerForAnotherTeam_ReturnsNull()
        {
            // Arrange

            using var context =
                CreateContext();

            var managerOne =
                new User
                {
                    Id = 1,
                    FullName =
                        "Manager One",

                    Email =
                        "manager1@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.Manager
                };

            var managerTwo =
                new User
                {
                    Id = 2,
                    FullName =
                        "Manager Two",

                    Email =
                        "manager2@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.Manager
                };

            context.Users.AddRange(
                managerOne,
                managerTwo);

            await context.SaveChangesAsync();

            var team =
                new Team
                {
                    Id = 1,
                    Name =
                        "Manager Two Team",

                    Description =
                        "Another manager team",

                    ManagerId =
                        managerTwo.Id,

                    CreatedAt =
                        DateTime.UtcNow
                };

            context.Teams.Add(team);

            await context.SaveChangesAsync();

            var service =
                CreateService(context);

            var dto =
                new CreateTaskDto
                {
                    Title =
                        "Unauthorized Task",

                    Description =
                        "Should not be created",

                    Priority =
                        TaskPriority.High,

                    Deadline =
                        DateTime.UtcNow.AddDays(2),

                    TeamId =
                        team.Id
                };

            // Act

            var result =
                await service.CreateTaskAsync(
                    dto,
                    managerOne.Id,
                    "Manager");

            // Assert

            Assert.Null(result);

            Assert.Empty(
                await context.Tasks.ToListAsync());
        }

        // ==========================================
        // TEST 5
        // USER CANNOT UPDATE ANOTHER USER'S TASK
        // ==========================================

        [Fact]
        public async Task
            UpdateStatus_ByUnassignedUser_ReturnsFalse()
        {
            // Arrange

            using var context =
                CreateContext();

            var creator =
                new User
                {
                    Id = 1,
                    FullName =
                        "Admin User",

                    Email =
                        "admin@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.Admin
                };

            var assignedUser =
                new User
                {
                    Id = 2,
                    FullName =
                        "Assigned User",

                    Email =
                        "assigned@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.User
                };

            var anotherUser =
                new User
                {
                    Id = 3,
                    FullName =
                        "Another User",

                    Email =
                        "another@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.User
                };

            context.Users.AddRange(
                creator,
                assignedUser,
                anotherUser);

            await context.SaveChangesAsync();

            var task =
                new TaskItem
                {
                    Id = 1,

                    Title =
                        "Protected Task",

                    Description =
                        "Assigned task",

                    Priority =
                        TaskPriority.Medium,

                    Status =
                        TaskStatusEnum.ToDo,

                    AssignedToUserId =
                        assignedUser.Id,

                    CreatedByUserId =
                        creator.Id,

                    CreatedAt =
                        DateTime.UtcNow
                };

            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service =
                CreateService(context);

            var dto =
                new UpdateTaskStatusDto
                {
                    Status =
                        TaskStatusEnum.InProgress
                };

            // Act

            var result =
                await service
                    .UpdateTaskStatusAsync(
                        task.Id,
                        dto,
                        anotherUser.Id,
                        "User");

            // Assert

            Assert.False(result);

            var savedTask =
                await context.Tasks
                    .FindAsync(task.Id);

            Assert.NotNull(savedTask);

            Assert.Equal(
                TaskStatusEnum.ToDo,
                savedTask.Status);
        }

        // ==========================================
        // TEST 6
        // ASSIGNED USER CAN UPDATE STATUS
        // ==========================================

        [Fact]
        public async Task
            UpdateStatus_ByAssignedUser_UpdatesTask()
        {
            // Arrange

            using var context =
                CreateContext();

            var creator =
                new User
                {
                    Id = 1,
                    FullName =
                        "Admin User",

                    Email =
                        "admin@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.Admin
                };

            var assignedUser =
                new User
                {
                    Id = 2,
                    FullName =
                        "Assigned User",

                    Email =
                        "user@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.User
                };

            context.Users.AddRange(
                creator,
                assignedUser);

            await context.SaveChangesAsync();

            var task =
                new TaskItem
                {
                    Id = 1,

                    Title =
                        "Status Test",

                    Description =
                        "Test Description",

                    Priority =
                        TaskPriority.Medium,

                    Status =
                        TaskStatusEnum.ToDo,

                    AssignedToUserId =
                        assignedUser.Id,

                    CreatedByUserId =
                        creator.Id,

                    CreatedAt =
                        DateTime.UtcNow
                };

            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service =
                CreateService(context);

            var dto =
                new UpdateTaskStatusDto
                {
                    Status =
                        TaskStatusEnum.InProgress
                };

            // Act

            var result =
                await service
                    .UpdateTaskStatusAsync(
                        task.Id,
                        dto,
                        assignedUser.Id,
                        "User");

            // Assert

            Assert.True(result);

            var savedTask =
                await context.Tasks
                    .FindAsync(task.Id);

            Assert.NotNull(savedTask);

            Assert.Equal(
                TaskStatusEnum.InProgress,
                savedTask.Status);

            // Verify activity history

            var activity =
                await context.TaskActivities
                    .FirstOrDefaultAsync(
                        a =>
                            a.TaskId ==
                            task.Id &&
                            a.Action ==
                            "StatusChanged");

            Assert.NotNull(activity);

            Assert.Equal(
                "User",
                activity.PerformedByRole);
        }

        // ==========================================
        // TEST 7
        // DONE TASK CANNOT RETURN
        // TO IN PROGRESS
        // ==========================================

        [Fact]
        public async Task
            UpdateStatus_FromDoneToInProgress_ReturnsFalse()
        {
            // Arrange

            using var context =
                CreateContext();

            var creator =
                new User
                {
                    Id = 1,
                    FullName =
                        "Admin User",

                    Email =
                        "admin@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.Admin
                };

            var assignedUser =
                new User
                {
                    Id = 2,
                    FullName =
                        "Assigned User",

                    Email =
                        "user@test.com",

                    PasswordHash =
                        "hash",

                    Role =
                        UserRole.User
                };

            context.Users.AddRange(
                creator,
                assignedUser);

            await context.SaveChangesAsync();

            var task =
                new TaskItem
                {
                    Id = 1,

                    Title =
                        "Completed Task",

                    Description =
                        "Already completed",

                    Priority =
                        TaskPriority.High,

                    Status =
                        TaskStatusEnum.Done,

                    AssignedToUserId =
                        assignedUser.Id,

                    CreatedByUserId =
                        creator.Id,

                    CreatedAt =
                        DateTime.UtcNow
                };

            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service =
                CreateService(context);

            var dto =
                new UpdateTaskStatusDto
                {
                    Status =
                        TaskStatusEnum.InProgress
                };

            // Act

            var result =
                await service
                    .UpdateTaskStatusAsync(
                        task.Id,
                        dto,
                        assignedUser.Id,
                        "User");

            // Assert

            Assert.False(result);

            var savedTask =
                await context.Tasks
                    .FindAsync(task.Id);

            Assert.NotNull(savedTask);

            Assert.Equal(
                TaskStatusEnum.Done,
                savedTask.Status);
        }
    }
}
