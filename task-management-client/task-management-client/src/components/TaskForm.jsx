import { useEffect, useState } from "react";

import {
  createTask,
  updateTask,
} from "../services/taskService";

import { getTeams } from "../services/teamService";
import { getUsers } from "../services/userService";

const initialForm = {
  title: "",
  description: "",
  priority: 2,
  deadline: "",
  teamId: "",
  assignedToUserId: "",
};

const TaskForm = ({
  task,
  onSuccess,
  onCancel,
}) => {
  const [form, setForm] =
    useState(initialForm);

  const [teams, setTeams] =
    useState([]);

  const [users, setUsers] =
    useState([]);

  const [loadingData, setLoadingData] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [error, setError] =
    useState("");

  const isEdit = Boolean(task);

  // ==========================================
  // Load Teams and Users
  // ==========================================

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoadingData(true);
        setError("");

        const [
          teamsData,
          usersData,
        ] = await Promise.all([
          getTeams(),
          getUsers(),
        ]);

        setTeams(
          Array.isArray(teamsData)
            ? teamsData
            : []
        );

        setUsers(
          Array.isArray(usersData)
            ? usersData
            : []
        );
      } catch (err) {
        console.error(
          "Error loading task form data:",
          err
        );

        setError(
          err.response?.data?.message ||
            "Unable to load teams and users."
        );
      } finally {
        setLoadingData(false);
      }
    };

    loadData();
  }, []);

  // ==========================================
  // Load Existing Task When Editing
  // ==========================================

  useEffect(() => {
    if (task) {
      let priorityValue = 2;

      if (task.priority === "Low") {
        priorityValue = 1;
      }

      if (task.priority === "Medium") {
        priorityValue = 2;
      }

      if (task.priority === "High") {
        priorityValue = 3;
      }

      setForm({
        title:
          task.title || "",

        description:
          task.description || "",

        priority:
          priorityValue,

        deadline:
          task.deadline
            ? task.deadline.substring(
                0,
                10
              )
            : "",

        teamId:
          task.teamId ?? "",

        assignedToUserId:
          task.assignedToUserId ?? "",
      });
    } else {
      setForm(initialForm);
    }
  }, [task]);

  // ==========================================
  // Normal Input Change
  // ==========================================

  const handleChange = (e) => {
    const {
      name,
      value,
    } = e.target;

    setForm((current) => ({
      ...current,
      [name]: value,
    }));
  };

  // ==========================================
  // Team Change
  // ==========================================

  const handleTeamChange = (e) => {
    const selectedTeamId =
      e.target.value;

    setForm((current) => ({
      ...current,

      teamId:
        selectedTeamId,

      // Clear assignee whenever
      // the team changes.
      assignedToUserId: "",
    }));
  };

  // ==========================================
  // Users For Selected Team
  // ==========================================

  const usersForSelectedTeam =
    users.filter((user) => {
      if (!form.teamId) {
        return true;
      }

      /*
       * This assumes GET /api/Users
       * returns teamId.
       *
       * If your UserDto uses another
       * property for team membership,
       * we will adjust this later.
       */
      return (
        Number(user.teamId) ===
        Number(form.teamId)
      );
    });

  // ==========================================
  // Form Validation
  // ==========================================

  const validateForm = () => {
    if (!form.title.trim()) {
      setError(
        "Task title is required."
      );

      return false;
    }

    if (
      form.title.trim().length > 200
    ) {
      setError(
        "Task title cannot exceed 200 characters."
      );

      return false;
    }

    if (
      form.description.trim().length >
      1000
    ) {
      setError(
        "Description cannot exceed 1000 characters."
      );

      return false;
    }

    if (!form.teamId) {
      setError(
        "Please select a team."
      );

      return false;
    }

    return true;
  };

  // ==========================================
  // Submit Create / Edit
  // ==========================================

  const handleSubmit = async (e) => {
    e.preventDefault();

    setError("");

    if (!validateForm()) {
      return;
    }

    try {
      setSaving(true);

      const payload = {
        title:
          form.title.trim(),

        description:
          form.description.trim() ||
          null,

        priority:
          Number(form.priority),

        deadline:
          form.deadline
            ? `${form.deadline}T23:59:59`
            : null,

        teamId:
          form.teamId
            ? Number(form.teamId)
            : null,

        assignedToUserId:
          form.assignedToUserId
            ? Number(
                form.assignedToUserId
              )
            : null,
      };

      console.log(
        "Task Payload:",
        payload
      );

      if (isEdit) {
        await updateTask(
          task.id,
          payload
        );
      } else {
        await createTask(
          payload
        );
      }

      if (onSuccess) {
        await onSuccess();
      }
    } catch (err) {
      console.error(
        "Error saving task:",
        err
      );

      let errorMessage =
        "Unable to save task.";

      if (
        typeof err.response?.data ===
        "string"
      ) {
        errorMessage =
          err.response.data;
      } else if (
        err.response?.data?.message
      ) {
        errorMessage =
          err.response.data.message;
      } else if (
        err.response?.data?.title
      ) {
        errorMessage =
          err.response.data.title;
      }

      setError(errorMessage);
    } finally {
      setSaving(false);
    }
  };

  // ==========================================
  // Loading
  // ==========================================

  if (loadingData) {
    return (
      <div className="modal-overlay">
        <div className="task-modal">

          <div className="page-message">
            Loading task form...
          </div>

        </div>
      </div>
    );
  }
  const today = new Date()
  .toISOString()
  .split("T")[0];

  // ==========================================
  // UI
  // ==========================================

  return (
    <div
      className="modal-overlay"
      onMouseDown={(e) => {
        if (
          e.target === e.currentTarget &&
          !saving
        ) {
          onCancel?.();
        }
      }}
    >
      <div className="task-modal">

        {/* Header */}

        <div className="modal-header">

          <div>
            <h2>
              {isEdit
                ? "Edit Task"
                : "Create Task"}
            </h2>

            <p className="modal-subtitle">
              {isEdit
                ? "Update task information"
                : "Create and assign a new task"}
            </p>
          </div>

          <button
            type="button"
            className="close-button"
            onClick={onCancel}
            disabled={saving}
            aria-label="Close"
          >
            ×
          </button>

        </div>

        {/* Error */}

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        {/* Form */}

        <form onSubmit={handleSubmit}>

          {/* Title */}

          <div className="form-group">

            <label htmlFor="task-title">
              Task Title *
            </label>

            <input
              id="task-title"
              type="text"
              name="title"
              value={form.title}
              onChange={handleChange}
              placeholder="Enter task title"
              maxLength={200}
              required
              disabled={saving}
            />

          </div>

          {/* Description */}

          <div className="form-group">

            <label htmlFor="task-description">
              Description
            </label>

            <textarea
              id="task-description"
              name="description"
              value={form.description}
              onChange={handleChange}
              placeholder="Enter task description"
              rows="4"
              maxLength={1000}
              disabled={saving}
            />

            <small>
              {form.description.length}
              /1000
            </small>

          </div>

          {/* Priority + Deadline */}

          <div className="form-row">

            <div className="form-group">

              <label htmlFor="task-priority">
                Priority *
              </label>

              <select
                id="task-priority"
                name="priority"
                value={form.priority}
                onChange={handleChange}
                disabled={saving}
                required
              >
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

            <div className="form-group">

              <label htmlFor="task-deadline">
                Deadline
              </label>

              <input
                id="task-deadline"
                type="date"
                name="deadline"
                value={form.deadline}
                min={today}
                onChange={handleChange}
                disabled={saving}
              />

            </div>

          </div>

          {/* Team + Assignee */}

          <div className="form-row">

            <div className="form-group">

              <label htmlFor="task-team">
                Team *
              </label>

              <select
                id="task-team"
                name="teamId"
                value={form.teamId}
                onChange={handleTeamChange}
                disabled={saving}
                required
              >
                <option value="">
                  Select Team
                </option>

                {teams.map((team) => (
                  <option
                    key={team.id}
                    value={team.id}
                  >
                    {team.name}
                  </option>
                ))}

              </select>

            </div>

            <div className="form-group">

              <label htmlFor="task-assignee">
                Assign To
              </label>

              <select
                id="task-assignee"
                name="assignedToUserId"
                value={
                  form.assignedToUserId
                }
                onChange={handleChange}
                disabled={
                  saving ||
                  !form.teamId
                }
              >
                <option value="">
                  {form.teamId
                    ? "Select User"
                    : "Select Team First"}
                </option>

                {usersForSelectedTeam.map(
                  (user) => (
                    <option
                      key={user.id}
                      value={user.id}
                    >
                      {user.fullName}
                      {user.email
                        ? ` (${user.email})`
                        : ""}
                    </option>
                  )
                )}

              </select>

              {form.teamId &&
                usersForSelectedTeam.length ===
                  0 && (
                  <small>
                    No users found in this
                    team.
                  </small>
                )}

            </div>

          </div>

          {/* Buttons */}

          <div className="modal-actions">

            <button
              type="button"
              className="secondary-button"
              onClick={onCancel}
              disabled={saving}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="primary-button"
              disabled={saving}
            >
              {saving
                ? "Saving..."
                : isEdit
                  ? "Update Task"
                  : "Create Task"}
            </button>

          </div>

        </form>

      </div>
    </div>
  );
};

export default TaskForm;