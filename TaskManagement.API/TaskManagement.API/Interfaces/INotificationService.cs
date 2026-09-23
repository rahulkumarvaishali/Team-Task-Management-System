using TaskManagement.API.DTOs.Notifications;

namespace TaskManagement.API.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(
            int userId,
            string message,
            string type);

        Task<IEnumerable<NotificationDto>>
            GetUserNotificationsAsync(int userId);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId);

        Task<bool> MarkAllAsReadAsync(int userId);
    }
}
