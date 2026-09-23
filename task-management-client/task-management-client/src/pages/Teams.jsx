import {
  useCallback,
  useEffect,
  useState,
} from "react";

import {
  getTeams,
  deleteTeam,
} from "../services/teamService";

import { useAuth } from "../context/AuthContext";
import TeamForm from "../components/TeamForm";
import TeamMembersModal  from "../components/TeamMembersModal";
const Teams = () => {
  const { user } = useAuth();

  const [teams, setTeams] =
    useState([]);

    const [  selectedTeam,  setSelectedTeam,] = useState(null);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [showTeamForm, setShowTeamForm] =
    useState(false);

  const [editingTeam, setEditingTeam] =
    useState(null);

  const isAdmin =
    user?.role === "Admin";

  // ================================
  // Load Teams
  // ================================

  const loadTeams =
    useCallback(async () => {
      try {
        setLoading(true);
        setError("");

        const data =
          await getTeams();

        setTeams(
          Array.isArray(data)
            ? data
            : []
        );
      } catch (err) {
        console.error(
          "Error loading teams:",
          err
        );

        setError(
          err.response?.data?.message ||
            "Unable to load teams."
        );
      } finally {
        setLoading(false);
      }
    }, []);

  useEffect(() => {
    loadTeams();
  }, [loadTeams]);

  // ================================
  // Open Create
  // ================================

  const handleCreate = () => {
    setEditingTeam(null);
    setShowTeamForm(true);
  };

  // ================================
  // Open Edit
  // ================================

  const handleEdit = (team) => {
    setEditingTeam(team);
    setShowTeamForm(true);
  };

  // ================================
  // Delete
  // ================================

  const handleDelete = async (
    teamId
  ) => {
    const confirmed =
      window.confirm(
        "Are you sure you want to delete this team?"
      );

    if (!confirmed) {
      return;
    }

    try {
      await deleteTeam(teamId);

      await loadTeams();
    } catch (err) {
      console.error(
        "Delete team failed:",
        err
      );

      alert(
        err.response?.data?.message ||
          "Unable to delete team."
      );
    }
  };

  return (
    <div>

      {/* Header */}

      <div className="page-header page-header-row">

        <div>
          <h1>Teams</h1>

          <p>
            Manage teams and team members
          </p>
        </div>

        {isAdmin && (
          <button
            type="button"
            className="primary-button"
            onClick={handleCreate}
          >
            + Create Team
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
          Loading teams...
        </div>
      ) : (
        <div className="table-container">

          <table className="data-table">

            <thead>
              <tr>
                <th>Team</th>
                <th>Manager</th>
                <th>Members</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>

              {teams.length === 0 ? (
                <tr>
                  <td
                    colSpan="5"
                    className="empty-table"
                  >
                    No teams found.
                  </td>
                </tr>
              ) : (
                teams.map((team) => (
                  <tr key={team.id}>

                    <td>
                      <strong>
                        {team.name}
                      </strong>

                      {team.description && (
                        <div className="task-description">
                          {team.description}
                        </div>
                      )}
                    </td>

                    <td>
                      {team.managerName ||
                        "No Manager"}
                    </td>

                    <td>
                      {team.memberCount ??
                        team.members?.length ??
                        0}
                    </td>

                    <td>
                      {team.createdAt
                        ? new Date(
                            team.createdAt
                          ).toLocaleDateString()
                        : "-"}
                    </td>

                    <td>
                      <div className="table-actions">

                        <button
                            type="button"
                            className="view-button"
                            onClick={() =>
                                setSelectedTeam(team)
                            }
                            >
                            Members
                            </button>

                        {isAdmin && (
                          <>
                            <button
                              type="button"
                              className="edit-button"
                              onClick={() =>
                                handleEdit(team)
                              }
                            >
                              Edit
                            </button>

                            <button
                              type="button"
                              className="delete-button"
                              onClick={() =>
                                handleDelete(
                                  team.id
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

      {/* Create/Edit Modal */}

      {showTeamForm && (
  <TeamForm
    team={editingTeam}
    onCancel={() => {
      setShowTeamForm(false);
      setEditingTeam(null);
    }}
    onSuccess={async () => {
      setShowTeamForm(false);
      setEditingTeam(null);

      await loadTeams();
    }}
  />
)}

{selectedTeam && (
  <TeamMembersModal
    team={selectedTeam}
    onClose={() =>
      setSelectedTeam(null)
    }
    onSuccess={async () => {
      await loadTeams();
    }}
  />
)}

    </div>
  );
};

export default Teams;