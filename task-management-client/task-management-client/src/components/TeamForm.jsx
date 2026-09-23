import { useEffect, useState } from "react";

import {
  createTeam,
  updateTeam,
} from "../services/teamService";

import { getUsers } from "../services/userService";

const TeamForm = ({
  team,
  onSuccess,
  onCancel,
}) => {
  const [form, setForm] = useState({
    name: "",
    description: "",
    managerId: "",
  });

  const [users, setUsers] = useState([]);
  const [loadingUsers, setLoadingUsers] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [error, setError] =
    useState("");

  const isEdit = Boolean(team);

  // Load users
  useEffect(() => {
    const loadUsers = async () => {
      try {
        setLoadingUsers(true);

        const data = await getUsers();

        setUsers(
          Array.isArray(data)
            ? data
            : []
        );
      } catch (err) {
        console.error(
          "Unable to load users:",
          err
        );

        setError(
          "Unable to load users."
        );
      } finally {
        setLoadingUsers(false);
      }
    };

    loadUsers();
  }, []);

  // Load existing team for Edit
  useEffect(() => {
    if (team) {
      setForm({
        name: team.name || "",

        description:
          team.description || "",

        managerId:
          team.managerId ?? "",
      });
    } else {
      setForm({
        name: "",
        description: "",
        managerId: "",
      });
    }
  }, [team]);

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

  const handleSubmit = async (e) => {
    e.preventDefault();

    setError("");

    if (!form.name.trim()) {
      setError(
        "Team name is required."
      );

      return;
    }

    try {
      setSaving(true);

      const payload = {
        name: form.name.trim(),

        description:
          form.description.trim() ||
          null,

        managerId:
          form.managerId
            ? Number(form.managerId)
            : null,
      };

      console.log(
        "Team Payload:",
        payload
      );

      if (isEdit) {
        await updateTeam(
          team.id,
          payload
        );
      } else {
        await createTeam(payload);
      }

      if (onSuccess) {
        await onSuccess();
      }
    } catch (err) {
      console.error(
        "Team save failed:",
        err
      );

      let message =
        "Unable to save team.";

      if (
        typeof err.response?.data ===
        "string"
      ) {
        message =
          err.response.data;
      } else if (
        err.response?.data?.message
      ) {
        message =
          err.response.data.message;
      } else if (
        err.response?.data?.title
      ) {
        message =
          err.response.data.title;
      }

      setError(message);
    } finally {
      setSaving(false);
    }
  };

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

        <div className="modal-header">

          <div>
            <h2>
              {isEdit
                ? "Edit Team"
                : "Create Team"}
            </h2>

            <p className="modal-subtitle">
              {isEdit
                ? "Update team information"
                : "Create a new team"}
            </p>
          </div>

          <button
            type="button"
            className="close-button"
            onClick={onCancel}
            disabled={saving}
          >
            ×
          </button>

        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>

          <div className="form-group">

            <label htmlFor="team-name">
              Team Name *
            </label>

            <input
              id="team-name"
              type="text"
              name="name"
              value={form.name}
              onChange={handleChange}
              placeholder="Enter team name"
              required
              disabled={saving}
            />

          </div>

          <div className="form-group">

            <label htmlFor="team-description">
              Description
            </label>

            <textarea
              id="team-description"
              name="description"
              value={form.description}
              onChange={handleChange}
              placeholder="Enter team description"
              rows="4"
              disabled={saving}
            />

          </div>

          <div className="form-group">

            <label htmlFor="team-manager">
              Manager *
            </label>

            <select
              id="team-manager"
              name="managerId"
              value={form.managerId}
              onChange={handleChange}
              required
              disabled={
                saving ||
                loadingUsers
              }
            >
              <option value="">
                {loadingUsers
                  ? "Loading users..."
                  : "Select Manager"}
              </option>

              {users
                .filter(
                  (item) =>
                    item.role ===
                      "Manager" ||
                    item.role === 2
                )
                .map((item) => (
                  <option
                    key={item.id}
                    value={item.id}
                  >
                    {item.fullName}
                    {item.email
                      ? ` (${item.email})`
                      : ""}
                  </option>
                ))}

            </select>

          </div>

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
                  ? "Update Team"
                  : "Create Team"}
            </button>

          </div>

        </form>

      </div>
    </div>
  );
};

export default TeamForm;