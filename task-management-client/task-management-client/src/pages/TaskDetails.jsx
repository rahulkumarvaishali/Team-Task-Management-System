import {
  useCallback,
  useEffect,
  useState,
} from "react";

import {
  Link,
  useParams,
} from "react-router-dom";

import {
  getTaskById,
  updateTaskStatus,
  getTaskActivity,
} from "../services/taskService";

import {
  getComments,
  addComment,
  deleteComment,
} from "../services/commentService";

import { useAuth } from "../context/AuthContext";

const TaskDetails = () => {
  const { id } = useParams();

  const { user } = useAuth();

  const [task, setTask] =
    useState(null);

  const [comments, setComments] =
    useState([]);

  // Admin-only activity history
  const [activities, setActivities] =
    useState([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [commentError, setCommentError] =
    useState("");

  const [activityError, setActivityError] =
    useState("");

  const [newComment, setNewComment] =
    useState("");

  const [addingComment, setAddingComment] =
    useState(false);

  const [updatingStatus, setUpdatingStatus] =
    useState(false);

  // ==========================================
  // Load Task
  // ==========================================

  const loadTask = useCallback(async () => {
    try {
      const data =
        await getTaskById(id);

      setTask(data);
    } catch (err) {
      console.error(
        "Unable to load task:",
        err
      );

      console.error(
        "Task API response:",
        err.response?.data
      );

      setError(
        err.response?.data?.message ||
          "Unable to load task details."
      );
    }
  }, [id]);

  // ==========================================
  // Load Comments
  // ==========================================

  const loadComments =
    useCallback(async () => {
      try {
        setCommentError("");

        const data =
          await getComments(id);

        setComments(
          Array.isArray(data)
            ? data
            : []
        );
      } catch (err) {
        console.error(
          "Unable to load comments:",
          err
        );

        console.error(
          "Comments status:",
          err.response?.status
        );

        console.error(
          "Comments response:",
          err.response?.data
        );

        setComments([]);

        setCommentError(
          "Unable to load comments."
        );
      }
    }, [id]);

  // ==========================================
  // Load Activity - ADMIN ONLY
  // ==========================================

  const loadActivity =
    useCallback(async () => {
      // Manager/User should not call
      // the Admin-only API.
      if (user?.role !== "Admin") {
        setActivities([]);
        return;
      }

      try {
        setActivityError("");

        const data =
          await getTaskActivity(id);

        setActivities(
          Array.isArray(data)
            ? data
            : []
        );
      } catch (err) {
        console.error(
          "Unable to load activity:",
          err
        );

        console.error(
          "Activity status:",
          err.response?.status
        );

        console.error(
          "Activity response:",
          err.response?.data
        );

        setActivities([]);

        setActivityError(
          err.response?.data?.message ||
            "Unable to load activity history."
        );
      }
    }, [id, user?.role]);

  // ==========================================
  // Initial Load
  // ==========================================

  useEffect(() => {
    const loadPage = async () => {
      try {
        setLoading(true);
        setError("");

        // Load task first.
        await loadTask();

        // Comments failure will not
        // hide task details.
        await loadComments();

        // Only Admin loads history.
        if (user?.role === "Admin") {
          await loadActivity();
        }
      } finally {
        setLoading(false);
      }
    };

    loadPage();
  }, [
    loadTask,
    loadComments,
    loadActivity,
    user?.role,
  ]);

  // ==========================================
  // Status Value
  // ==========================================

  const getStatusValue = (status) => {
    if (
      status === "ToDo" ||
      status === 1
    ) {
      return 1;
    }

    if (
      status === "InProgress" ||
      status === 2
    ) {
      return 2;
    }

    if (
      status === "Done" ||
      status === 3
    ) {
      return 3;
    }

    return 1;
  };

  // ==========================================
  // Is Task Done?
  // ==========================================

  const isTaskDone =
    task?.status === "Done" ||
    task?.status === 3;

  // ==========================================
  // Is Task Overdue?
  // ==========================================

  const isOverdue =
    !!task?.deadline &&
    !isTaskDone &&
    new Date(task.deadline) <
      new Date();

  // ==========================================
  // Status Update
  // ==========================================

  const handleStatusChange = async (
    e
  ) => {
    const status =
      Number(e.target.value);

    try {
      setUpdatingStatus(true);

      await updateTaskStatus(
        id,
        status
      );

      await loadTask();

      // If Admin changed status,
      // immediately refresh history.
      if (user?.role === "Admin") {
        await loadActivity();
      }
    } catch (err) {
      console.error(
        "Unable to update status:",
        err
      );

      alert(
        err.response?.data?.message ||
          "Unable to update task status."
      );
    } finally {
      setUpdatingStatus(false);
    }
  };

  // ==========================================
  // Add Comment
  // ==========================================

  const handleAddComment = async (
    e
  ) => {
    e.preventDefault();

    const content =
      newComment.trim();

    if (!content) {
      return;
    }

    try {
      setAddingComment(true);
      setCommentError("");

      await addComment(
        id,
        content
      );

      setNewComment("");

      await loadComments();
    } catch (err) {
      console.error(
        "Unable to add comment:",
        err
      );

      setCommentError(
        err.response?.data?.message ||
          "Unable to add comment."
      );
    } finally {
      setAddingComment(false);
    }
  };

  // ==========================================
  // Delete Comment
  // ==========================================

  const handleDeleteComment = async (
    commentId
  ) => {
    const confirmed =
      window.confirm(
        "Delete this comment?"
      );

    if (!confirmed) {
      return;
    }

    try {
      await deleteComment(
        commentId
      );

      await loadComments();
    } catch (err) {
      console.error(
        "Unable to delete comment:",
        err
      );

      alert(
        err.response?.data?.message ||
          "Unable to delete comment."
      );
    }
  };

  // ==========================================
  // Loading
  // ==========================================

  if (loading) {
    return (
      <div className="page-message">
        Loading task details...
      </div>
    );
  }

  // ==========================================
  // Task Error
  // ==========================================

  if (error || !task) {
    return (
      <div>

        <Link
          to="/tasks"
          className="back-link"
        >
          ← Back to Tasks
        </Link>

        <div className="error-message">
          {error ||
            "Task not found."}
        </div>

      </div>
    );
  }

  // ==========================================
  // UI
  // ==========================================

  return (
    <div>

      <Link
        to="/tasks"
        className="back-link"
      >
        ← Back to Tasks
      </Link>

      {/* ======================================
          TASK DETAILS
      ====================================== */}

      <div className="details-card">

        <div className="page-header">

          <h1>
            {task.title}
          </h1>

          <p>
            Task #{task.id}
          </p>

        </div>

        {/* Overdue Warning */}

        {isOverdue && (
          <div className="overdue-alert">
            ⚠ This task is overdue.
            The deadline has passed and
            the task is not completed.
          </div>
        )}

        <div className="details-grid">

          {/* Status */}

          <div className="detail-item">

            <span>Status</span>

            <div>

              <select
                className="status-select"
                value={getStatusValue(
                  task.status
                )}
                onChange={
                  handleStatusChange
                }
                disabled={
                  updatingStatus ||
                  isTaskDone
                }
              >
                <option value={1}>
                  To Do
                </option>

                <option value={2}>
                  In Progress
                </option>

                <option value={3}>
                  Done
                </option>

              </select>

              {isTaskDone && (
                <small className="status-note">
                  Completed
                </small>
              )}

              {isOverdue && (
                <small className="overdue-text">
                  Overdue
                </small>
              )}

            </div>

          </div>

          {/* Priority */}

          <div className="detail-item">

            <span>Priority</span>

            <strong>
              {task.priority}
            </strong>

          </div>

          {/* Deadline */}

          <div className="detail-item">

            <span>Deadline</span>

            <strong>
              {task.deadline
                ? new Date(
                    task.deadline
                  ).toLocaleString()
                : "No deadline"}
            </strong>

            {isOverdue && (
              <small className="overdue-text">
                Deadline passed
              </small>
            )}

          </div>

          {/* Team */}

          <div className="detail-item">

            <span>Team</span>

            <strong>
              {task.teamName ||
                "No Team"}
            </strong>

          </div>

          {/* Assigned To */}

          <div className="detail-item">

            <span>
              Assigned To
            </span>

            <strong>
              {task.assignedToUserName ||
                "Unassigned"}
            </strong>

          </div>

          {/* Created By */}

          <div className="detail-item">

            <span>
              Created By
            </span>

            <strong>
              {task.createdByUserName ||
                "-"}
            </strong>

          </div>

        </div>

        {/* Description */}

        <div className="description-section">

          <h3>Description</h3>

          <p>
            {task.description ||
              "No description provided."}
          </p>

        </div>

        {/* Dates */}

        <div className="task-meta">

          <span>
            Created:{" "}
            {task.createdAt
              ? new Date(
                  task.createdAt
                ).toLocaleString()
              : "-"}
          </span>

          {task.updatedAt && (
            <span>
              Updated:{" "}
              {new Date(
                task.updatedAt
              ).toLocaleString()}
            </span>
          )}

        </div>

      </div>

      {/* ======================================
          COMMENTS
      ====================================== */}

      <div className="comments-card">

        <h2>
          Comments
        </h2>

        {commentError && (
          <div className="error-message">
            {commentError}
          </div>
        )}

        {/* Add Comment */}

        <form
          className="comment-form"
          onSubmit={
            handleAddComment
          }
        >

          <textarea
            value={newComment}
            onChange={(e) =>
              setNewComment(
                e.target.value
              )
            }
            placeholder="Write a comment..."
            rows="4"
            maxLength={1000}
            disabled={
              addingComment
            }
          />

          <div className="comment-form-footer">

            <small>
              {newComment.length}
              /1000
            </small>

            <button
              type="submit"
              className="primary-button"
              disabled={
                addingComment ||
                !newComment.trim()
              }
            >
              {addingComment
                ? "Adding..."
                : "Add Comment"}
            </button>

          </div>

        </form>

        {/* Comment List */}

        {comments.length === 0 ? (
          <div className="empty-comments">
            No comments yet.
          </div>
        ) : (
          <div>

            {comments.map(
              (comment) => {

                const canDelete =
                  user?.role ===
                    "Admin" ||
                  Number(
                    comment.userId
                  ) ===
                    Number(
                      user?.userId
                    );

                return (
                  <div
                    className="comment-item"
                    key={comment.id}
                  >

                    <div className="comment-header">

                      <div>

                        <strong>
                          {comment.userName ||
                            "User"}
                        </strong>

                        <span className="comment-date">
                          {comment.createdAt
                            ? new Date(
                                comment.createdAt
                              ).toLocaleString()
                            : ""}
                        </span>

                      </div>

                      {canDelete && (
                        <button
                          type="button"
                          className="comment-delete-button"
                          onClick={() =>
                            handleDeleteComment(
                              comment.id
                            )
                          }
                        >
                          Delete
                        </button>
                      )}

                    </div>

                    <p>
                      {comment.content}
                    </p>

                  </div>
                );
              }
            )}

          </div>
        )}

      </div>

      {/* ======================================
          ACTIVITY HISTORY - ADMIN ONLY
      ====================================== */}

      {user?.role === "Admin" && (
        <div className="comments-card">

          <div className="activity-title">

            <div>
              <h2>
                Activity History
              </h2>

              <p>
                Task activity visible to
                administrators only.
              </p>
            </div>

          </div>

          {activityError && (
            <div className="error-message">
              {activityError}
            </div>
          )}

          {!activityError &&
          activities.length === 0 ? (
            <div className="empty-comments">
              No activity recorded yet.
            </div>
          ) : (
            <div className="activity-list">

              {activities.map(
                (activity) => (
                  <div
                    className="activity-item"
                    key={activity.id}
                  >

                    <div className="activity-dot">
                    </div>

                    <div className="activity-content">

                      <div className="activity-header">

                        <strong>
                          {activity.performedByName ||
                            "Unknown User"}
                        </strong>

                        <span className="activity-role">
                          {activity.performedByRole}
                        </span>

                      </div>

                      <p>
                        {activity.details}
                      </p>

                      <small>
                        {activity.createdAt
                          ? new Date(
                              activity.createdAt
                            ).toLocaleString()
                          : ""}
                      </small>

                    </div>

                  </div>
                )
              )}

            </div>
          )}

        </div>
      )}

    </div>
  );
};

export default TaskDetails;