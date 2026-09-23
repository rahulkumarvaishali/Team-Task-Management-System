using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.API.DTOs.Notifications;
using TaskManagement.API.Interfaces;

namespace TaskManagement.Tests.Helpers
{
    internal class FakeNotificationService : INotificationService
    {
        public List<NotificationDto> Notifications
        {
            get;
        } = new();

        public Task CreateNotificationAsync(
            int userId,
            string message,
            string type)
        {
            // For TaskService unit tests,
            // we don't need to save a real notification.

            return Task.CompletedTask;
        }

        public Task<IEnumerable<NotificationDto>>
            GetUserNotificationsAsync(int userId)
        {
            return Task.FromResult<
                IEnumerable<NotificationDto>>(
                    Notifications
                );
        }

        public Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> MarkAllAsReadAsync(
            int userId)
        {
            return Task.FromResult(true);
        }
    }
}
