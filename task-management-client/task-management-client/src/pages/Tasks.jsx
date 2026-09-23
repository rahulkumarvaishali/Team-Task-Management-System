import {
  useCallback,
  useEffect,
  useState,
} from "react";

import { useNavigate } from "react-router-dom";

import {
  getTasks,
  updateTaskStatus,
  deleteTask,
} from "../services/taskService";

import { useAuth } from "../context/AuthContext";
import TaskForm from "../components/TaskForm";

const Tasks = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [tasks, setTasks] = useState([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  // Create/Edit Task modal
  const [showTaskForm, setShowTaskForm] =
    useState(false);

  const [editingTask, setEditingTask] =
    useState(null);

  // Filters
  const [filters, setFilters] =
    useState({
      status: "",
      priority: "",
      deadline: "",
    });

  const canManageTasks =
    user?.role === "Admin" ||
    user?.role === "Manager";

  // ==========================================
  // Load Tasks
  // ==========================================

  const loadTasks = useCallback(async () => {
    try {
      setLoading(true);
      setError("");

      const params = {};

      if (filters.status) {
        params.status = filters.status;
      }

      if (filters.priority) {
        params.priority = filters.priority;
      }

      if (filters.deadline) {
        params.deadline = filters.deadline;
      }

      const data = await getTasks(params);

      setTasks(
        Array.isArray(data)
          ? data
          : []
      );
    } catch (err) {
      console.error(
        "Error loading tasks:",
        err
      );

      setError(
        err.response?.data?.message ||
          "Unable to load tasks."
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    loadTasks();
  }, [loadTasks]);

  // ==========================================
  // Filters
  // ==========================================

  const handleFilterChange = (e) => {
    const {
      name,
      value,
    } = e.target;

    setFilters((current) => ({
      ...current,
      [name]: value,
    }));
  };

  const clearFilters = () => {
    setFilters({
      status: "",
      priority: "",
      deadline: "",
    });
  };

  // ==========================================
  // Create Task
  // ==========================================

  const handleCreate = () => {
    console.log("Create Task clicked");

    setEditingTask(null);
    setShowTaskForm(true);
  };

  // ==========================================
  // Edit Task
  // ==========================================

  const handleEdit = (task) => {
    setEditingTask(task);
    setShowTaskForm(true);
  };

  // ==========================================
  // Status
  // ==========================================

  const handleStatusChange = async (
    taskId,
    status
  ) => {
    try {
      await updateTaskStatus(
        taskId,
        Number(status)
      );

      await loadTasks();
    } catch (err) {
      console.error(
        "Status update failed:",
        err
      );

      alert(
        err.response?.data?.message ||
          "Unable to update task status."
      );
    }
  };

  // ==========================================
  // Delete
  // ==========================================

  const handleDelete = async (
    taskId
  ) => {
    const confirmed =
      window.confirm(
        "Are you sure you want to delete this task?"
      );

    if (!confirmed) {
      return;
    }

    try {
      await deleteTask(taskId);

      await loadTasks();
    } catch (err) {
      console.error(
        "Delete task failed:",
        err
      );

      alert(
        err.response?.data?.message ||
          "Unable to delete task."
      );
    }
  };

  // ==========================================
  // Convert status string to enum number
  // ==========================================

  const getStatusValue = (status) => {
    if (status === "ToDo") {
      return 1;
    }

    if (status === "InProgress") {
      return 2;
    }

    if (status === "Done") {
      return 3;
    }

    return 1;
  };

  // ==========================================
  // UI
  // ==========================================

  return (
    <div>

      {/* Header */}

      <div className="page-header page-header-row">

        <div>
          <h1>Tasks</h1>

          <p>
            Manage and track your tasks
          </p>
        </div>

        {canManageTasks && (
          <button
            type="button"
            className="primary-button"
            onClick={handleCreate}
          >
            + Create Task
          </button>
        )}

      </div>

      {/* Filters */}

      <div className="filter-card">

        <div className="filter-group">
          <label>Status</label>

          <select
            name="status"
            value={filters.status}
            onChange={handleFilterChange}
          >
            <option value="">
              All Statuses
            </option>

            <option value="1">
              To Do
            </option>

            <option value="2">
              In Progress
            </option>

            <option value="3">
              Done
            </option>
          </select>
        </div>

        <div className="filter-group">
          <label>Priority</label>

          <select
            name="priority"
            value={filters.priority}
            onChange={handleFilterChange}
          >
            <option value="">
              All Priorities
            </option>

            <option value="1">
              Low
            </option>

            <option value="2">
              Medium
            </option>

            <option value="3">
              High
            </option>
          </select>
        </div>

        <div className="filter-group">
          <label>Deadline</label>

          <input
            type="date"
            name="deadline"
            value={filters.deadline}
            onChange={handleFilterChange}
          />
        </div>

        <button
          type="button"
          className="secondary-button"
          onClick={clearFilters}
        >
          Clear
        </button>

      </div>

      {/* Error */}

      {error && (
        <div className="error-message">
          {error}
        </div>
      )}

      {/* Tasks */}

      {loading ? (
        <div className="page-message">
          Loading tasks...
        </div>
      ) : (
        <div className="table-container">

          <table className="data-table">

            <thead>
              <tr>
                <th>Task</th>
                <th>Team</th>
                <th>Assigned To</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Deadline</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>

              {tasks.length === 0 ? (
                <tr>
                  <td
                    colSpan="7"
                    className="empty-table"
                  >
                    No tasks found.
                  </td>
                </tr>
              ) : (
                tasks.map((task) => (
                  <tr key={task.id}>

                    {/* Task */}

                    <td>
                      <strong>
                        {task.title}
                      </strong>

                      {task.description && (
                        <div className="task-description">
                          {task.description}
                        </div>
                      )}
                    </td>

                    {/* Team */}

                    <td>
                      {task.teamName ||
                        "-"}
                    </td>

                    {/* Assigned User */}

                    <td>
                      {task.assignedToUserName ||
                        "Unassigned"}
                    </td>

                    {/* Priority */}

                    <td>
                      <span
                        className={
                          `badge priority-${
                            task.priority
                              ?.toLowerCase()
                          }`
                        }
                      >
                        {task.priority}
                      </span>
                    </td>

                    {/* Status */}

                    <td>
                      <select
                        className="status-select"
                        value={getStatusValue(
                          task.status
                        )}
                        onChange={(e) =>
                          handleStatusChange(
                            task.id,
                            e.target.value
                          )
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
                    </td>

                    {/* Deadline */}

                    <td>
                      {task.deadline
                        ? new Date(
                            task.deadline
                          ).toLocaleDateString()
                        : "-"}
                    </td>

                    {/* Actions */}

                    <td>
                      <div className="table-actions">

                        <button
                          type="button"
                          className="view-button"
                          onClick={() =>
                            navigate(
                              `/tasks/${task.id}`
                            )
                          }
                        >
                          View
                        </button>

                        {canManageTasks && (
                          <>
                            <button
                              type="button"
                              className="edit-button"
                              onClick={() =>
                                handleEdit(
                                  task
                                )
                              }
                            >
                              Edit
                            </button>

                            <button
                              type="button"
                              className="delete-button"
                              onClick={() =>
                                handleDelete(
                                  task.id
                                )
                              }
                            >
                              Delete
                            </button>
                          </>
                        )}

                      </div>
                    </td>

                  </tr>
                ))
              )}

            </tbody>

          </table>

        </div>
      )}

      {/* ======================================
          CREATE / EDIT TASK MODAL
         ====================================== */}

      {showTaskForm && (
        <TaskForm
          task={editingTask}
          onCancel={() => {
            setShowTaskForm(false);
            setEditingTask(null);
          }}
          onSuccess={async () => {
            setShowTaskForm(false);
            setEditingTask(null);

            await loadTasks();
          }}
        />
      )}

    </div>
  );
};

export default Tasks;