import {
  useCallback,
  useEffect,
  useState,
} from "react";

import {
  getNotifications,
  markNotificationAsRead,
  markAllNotificationsAsRead,
} from "../services/notificationService";

const Notifications = () => {
  const [notifications, setNotifications] =
    useState([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [processingId, setProcessingId] =
    useState(null);

  const [markingAll, setMarkingAll] =
    useState(false);

  // ==========================================
  // Load Notifications
  // ==========================================

  const loadNotifications =
    useCallback(async () => {
      try {
        setLoading(true);
        setError("");

        const data =
          await getNotifications();

        setNotifications(
          Array.isArray(data)
            ? data
            : []
        );
      } catch (err) {
        console.error(
          "Unable to load notifications:",
          err
        );

        setError(
          err.response?.data?.message ||
            "Unable to load notifications."
        );
      } finally {
        setLoading(false);
      }
    }, []);

  useEffect(() => {
    loadNotifications();
  }, [loadNotifications]);

  // ==========================================
  // Mark One As Read
  // ==========================================

  const handleMarkAsRead = async (
    notificationId
  ) => {
    try {
      setProcessingId(notificationId);

      await markNotificationAsRead(
        notificationId
      );

      setNotifications((current) =>
        current.map((notification) =>
          notification.id === notificationId
            ? {
                ...notification,
                isRead: true,
              }
            : notification
        )
      );
    } catch (err) {
      console.error(
        "Unable to mark notification as read:",
        err
      );

      alert(
        err.response?.data?.message ||
          "Unable to mark notification as read."
      );
    } finally {
      setProcessingId(null);
    }
  };

  // ==========================================
  // Mark All As Read
  // ==========================================

  const handleMarkAllAsRead =
    async () => {
      try {
        setMarkingAll(true);

        await markAllNotificationsAsRead();

        setNotifications((current) =>
          current.map((notification) => ({
            ...notification,
            isRead: true,
          }))
        );
      } catch (err) {
        console.error(
          "Unable to mark all notifications:",
          err
        );

        alert(
          err.response?.data?.message ||
            "Unable to mark all notifications as read."
        );
      } finally {
        setMarkingAll(false);
      }
    };

  // ==========================================
  // Date Formatting
  // ==========================================

  const formatDate = (dateValue) => {
    if (!dateValue) {
      return "";
    }

    const date = new Date(dateValue);

    if (Number.isNaN(date.getTime())) {
      return "";
    }

    return date.toLocaleString();
  };

  // ==========================================
  // Notification Type
  // ==========================================

  const getTypeLabel = (type) => {
    if (type === "TaskAssigned") {
      return "Task Assigned";
    }

    if (type === "TaskStatusUpdated") {
      return "Task Status Updated";
    }

    return type || "Notification";
  };

  const unreadCount =
    notifications.filter(
      (notification) =>
        !notification.isRead
    ).length;

  // ==========================================
  // UI
  // ==========================================

  return (
    <div>

      {/* Header */}

      <div className="page-header page-header-row">

        <div>
          <h1>Notifications</h1>

          <p>
            You have {unreadCount} unread{" "}
            {unreadCount === 1
              ? "notification"
              : "notifications"}
          </p>
        </div>

        {unreadCount > 0 && (
          <button
            type="button"
            className="primary-button"
            onClick={
              handleMarkAllAsRead
            }
            disabled={markingAll}
          >
            {markingAll
              ? "Updating..."
              : "Mark All as Read"}
          </button>
        )}

      </div>

      {/* Error */}

      {error && (
        <div className="error-message">
          {error}
        </div>
      )}

      {/* Loading */}

      {loading ? (
        <div className="page-message">
          Loading notifications...
        </div>
      ) : notifications.length === 0 ? (
        <div className="empty-notifications">

          <div className="empty-notification-icon">
            🔔
          </div>

          <h3>
            No notifications
          </h3>

          <p>
            You don't have any notifications yet.
          </p>

        </div>
      ) : (
        <div className="notifications-list">

          {notifications.map(
            (notification) => (
              <div
                key={notification.id}
                className={
                  notification.isRead
                    ? "notification-card"
                    : "notification-card notification-unread"
                }
              >

                {/* Indicator */}

                <div className="notification-indicator">

                  {!notification.isRead && (
                    <span className="unread-dot" />
                  )}

                </div>

                {/* Content */}

                <div className="notification-content">

                  <div className="notification-top">

                    <span className="notification-type">
                      {getTypeLabel(
                        notification.type
                      )}
                    </span>

                    <span className="notification-date">
                      {formatDate(
                        notification.createdAt
                      )}
                    </span>

                  </div>

                  <p className="notification-message">
                    {notification.message}
                  </p>

                </div>

                {/* Action */}

                <div className="notification-action">

                  {!notification.isRead ? (
                    <button
                      type="button"
                      className="view-button"
                      disabled={
                        processingId ===
                        notification.id
                      }
                      onClick={() =>
                        handleMarkAsRead(
                          notification.id
                        )
                      }
                    >
                      {processingId ===
                      notification.id
                        ? "Updating..."
                        : "Mark as Read"}
                    </button>
                  ) : (
                    <span className="read-label">
                      ✓ Read
                    </span>
                  )}

                </div>

              </div>
            )
          )}

        </div>
      )}

    </div>
  );
};

export default Notifications;