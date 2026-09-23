import {
  useCallback,
  useEffect,
  useState,
} from "react";

import {
  getUsers,
  updateUserRole,
} from "../services/userService";

import { useAuth } from "../context/AuthContext";

const Users = () => {
  const { user: currentUser } =
    useAuth();

  const [users, setUsers] =
    useState([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [updatingUserId, setUpdatingUserId] =
    useState(null);

  const isAdmin =
    currentUser?.role === "Admin";

  // ==========================================
  // Load Users
  // ==========================================

  const loadUsers =
    useCallback(async () => {
      try {
        setLoading(true);
        setError("");

        const data =
          await getUsers();

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
    }, []);

  useEffect(() => {
    loadUsers();
  }, [loadUsers]);

  // ==========================================
  // Convert Role to Number
  // ==========================================

  const getRoleValue = (role) => {
    if (role === "Admin" || role === 1) {
      return 1;
    }

    if (
      role === "Manager" ||
      role === 2
    ) {
      return 2;
    }

    return 3;
  };

  // ==========================================
  // Display Role
  // ==========================================

  const getRoleName = (role) => {
    if (role === "Admin" || role === 1) {
      return "Admin";
    }

    if (
      role === "Manager" ||
      role === 2
    ) {
      return "Manager";
    }

    return "User";
  };

  // ==========================================
  // Update Role
  // ==========================================

  const handleRoleChange = async (
    targetUser,
    newRole
  ) => {
    const oldRole =
      getRoleValue(targetUser.role);

    const newRoleNumber =
      Number(newRole);

    if (oldRole === newRoleNumber) {
      return;
    }

    const roleName =
      getRoleName(newRoleNumber);

    const confirmed =
      window.confirm(
        `Change ${targetUser.fullName}'s role to ${roleName}?`
      );

    if (!confirmed) {
      return;
    }

    try {
      setUpdatingUserId(
        targetUser.id
      );

      await updateUserRole(
        targetUser.id,
        newRoleNumber
      );

      await loadUsers();
    } catch (err) {
      console.error(
        "Role update failed:",
        err
      );

      alert(
        err.response?.data?.message ||
          "Unable to update user role."
      );

      await loadUsers();
    } finally {
      setUpdatingUserId(null);
    }
  };

  // ==========================================
  // UI
  // ==========================================

  return (
    <div>

      <div className="page-header">

        <h1>Users</h1>

        <p>
          Manage system users and roles
        </p>

      </div>

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
        <div className="table-container">

          <table className="data-table">

            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Team</th>

                {isAdmin && (
                  <th>Change Role</th>
                )}
              </tr>
            </thead>

            <tbody>

              {users.length === 0 ? (
                <tr>
                  <td
                    colSpan={
                      isAdmin ? 5 : 4
                    }
                    className="empty-table"
                  >
                    No users found.
                  </td>
                </tr>
              ) : (
                users.map((user) => {

                  const isCurrentUser =
                    Number(user.id) ===
                    Number(
                      currentUser?.userId
                    );

                  return (
                    <tr key={user.id}>

                      {/* Name */}

                      <td>
                        <strong>
                          {user.fullName}
                        </strong>

                        {isCurrentUser && (
                          <span className="you-label">
                            You
                          </span>
                        )}
                      </td>

                      {/* Email */}

                      <td>
                        {user.email}
                      </td>

                      {/* Role */}

                      <td>
                        <span
                          className={`role-badge role-${getRoleName(
                            user.role
                          ).toLowerCase()}`}
                        >
                          {getRoleName(
                            user.role
                          )}
                        </span>
                      </td>

                      {/* Team */}

                      <td>
                        {user.teamName ||
                          "No Team"}
                      </td>

                      {/* Role Management */}

                      {isAdmin && (
                        <td>

                          {isCurrentUser ? (
                            <span className="muted-text">
                              Current account
                            </span>
                          ) : (
                            <select
                              className="role-select"
                              value={getRoleValue(
                                user.role
                              )}
                              disabled={
                                updatingUserId ===
                                user.id
                              }
                              onChange={(e) =>
                                handleRoleChange(
                                  user,
                                  e.target.value
                                )
                              }
                            >

                              <option value={1}>
                                Admin
                              </option>

                              <option value={2}>
                                Manager
                              </option>

                              <option value={3}>
                                User
                              </option>

                            </select>
                          )}

                        </td>
                      )}

                    </tr>
                  );
                })
              )}

            </tbody>

          </table>

        </div>
      )}

    </div>
  );
};

export default Users;