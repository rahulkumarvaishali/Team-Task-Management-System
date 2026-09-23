namespace TaskManagement.API.DTOs.Tasks
{
    public class DashboardDto
    {
        public int TotalTasks { get; set; }

        public int ToDoTasks { get; set; }

        public int InProgressTasks { get; set; }

        public int DoneTasks { get; set; }

        public int LowPriorityTasks { get; set; }

        public int MediumPriorityTasks { get; set; }

        public int HighPriorityTasks { get; set; }

        public int OverdueTasks { get; set; }
    }
}
