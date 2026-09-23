import api from "../api/axios";

// Get logged-in user's notifications
export const getNotifications = async () => {
  const response = await api.get(
    "/Notifications"
  );

  return response.data;
};

// Mark one notification as read
export const markNotificationAsRead = async (
  notificationId
) => {
  const response = await api.patch(
    `/Notifications/${notificationId}/read`
  );

  return response.data;
};

// Mark all notifications as read
export const markAllNotificationsAsRead =
  async () => {
    const response = await api.patch(
      "/Notifications/read-all"
    );

    return response.data;
  };