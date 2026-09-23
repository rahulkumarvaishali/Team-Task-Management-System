import {
  useEffect,
  useState,
} from "react";

import {
  addTeamMember,
  removeTeamMember,
} from "../services/teamService";

import { getUsers } from "../services/userService";

const TeamMembersModal = ({
  team,
  onClose,
  onSuccess,
}) => {
  const [users, setUsers] = useState([]);

  const [selectedUserId, setSelectedUserId] =
    useState("");

  const [loading, setLoading] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [error, setError] =
    useState("");

  // ==========================================
  // Load Users
  // ==========================================

  const loadUsers = async () => {
    try {
      setLoading(true);
      setError("");

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
        err.response?.data?.message ||
          "Unable to load users."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();
  }, []);

  // ==========================================
  // Determine Team Membership
  // ==========================================

  const currentMembers = users.filter(
    (user) =>
      Number(user.teamId) ===
      Number(team.id)
  );

  const availableUsers = users.filter(
    (user) =>
      Number(user.teamId) !==
      Number(team.id)
  );

  // ==========================================
  // Add Member
  // ==========================================

  const handleAddMember = async () => {
    if (!selectedUserId) {
      setError(
        "Please select a user."
      );

      return;
    }

    try {
      setSaving(true);
      setError("");

      await addTeamMember(
        team.id,
        Number(selectedUserId)
      );

      setSelectedUserId("");

      await loadUsers();

      if (onSuccess) {
        await onSuccess();
      }
    } catch (err) {
      console.error(
        "Add member failed:",
        err
      );

      setError(
        err.response?.data?.message ||
          "Unable to add member."
      );
    } finally {
      setSaving(false);
    }
  };

  // ==========================================
  // Remove Member
  // ==========================================

  const handleRemoveMember = async (
    userId
  ) => {
    const confirmed =
      window.confirm(
        "Remove this user from the team?"
      );

    if (!confirmed) {
      return;
    }

    try {
      setSaving(true);
      setError("");

      await removeTeamMember(
        team.id,
        userId
      );

      await loadUsers();

      if (onSuccess) {
        await onSuccess();
      }
    } catch (err) {
      console.error(
        "Remove member failed:",
        err
      );

      setError(
        err.response?.data?.message ||
          "Unable to remove member."
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="modal-overlay">

      <div className="task-modal">

        {/* Header */}

        <div className="modal-header">

          <div>
            <h2>Team Members</h2>

            <p className="modal-subtitle">
              {team.name}
            </p>
          </div>

          <button
            type="button"
            className="close-button"
            onClick={onClose}
            disabled={saving}
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

        {loading ? (
          <div className="page-message">
            Loading users...
          </div>
        ) : (
          <>

            {/* Current Members */}

            <div className="team-members-section">

              <h3>
                Current Members (
                {currentMembers.length})
              </h3>

              {currentMembers.length ===
              0 ? (
                <div className="empty-comments">
                  No members in this team.
                </div>
              ) : (
                <div className="members-list">

                  {currentMembers.map(
                    (member) => (
                      <div
                        className="member-row"
                        key={member.id}
                      >

                        <div>
                          <strong>
                            {member.fullName}
                          </strong>

                          <span>
                            {member.email}
                          </span>
                        </div>

                        <button
                          type="button"
                          className="delete-button"
                          disabled={saving}
                          onClick={() =>
                            handleRemoveMember(
                              member.id
                            )
                          }
                        >
                          Remove
                        </button>

                      </div>
                    )
                  )}

                </div>
              )}

            </div>

            {/* Add Member */}

            <div className="add-member-section">

              <h3>Add Member</h3>

              <div className="add-member-row">

                <select
                  value={selectedUserId}
                  onChange={(e) =>
                    setSelectedUserId(
                      e.target.value
                    )
                  }
                  disabled={saving}
                >

                  <option value="">
                    Select User
                  </option>

                  {availableUsers.map(
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

                <button
                  type="button"
                  className="primary-button"
                  onClick={
                    handleAddMember
                  }
                  disabled={
                    saving ||
                    !selectedUserId
                  }
                >
                  {saving
                    ? "Please wait..."
                    : "Add Member"}
                </button>

              </div>

            </div>

            {/* Footer */}

            <div className="modal-actions">

              <button
                type="button"
                className="secondary-button"
                onClick={onClose}
                disabled={saving}
              >
                Close
              </button>

            </div>

          </>
        )}

      </div>

    </div>
  );
};

export default TeamMembersModal;