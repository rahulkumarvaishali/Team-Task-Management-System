import { NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const Sidebar = () => {
  const { user } = useAuth();

  return (
    <aside className="sidebar">

      <div className="sidebar-logo">
        <h2>TaskFlow</h2>
      </div>

      <nav className="sidebar-nav">

        <NavLink to="/dashboard">
          Dashboard
        </NavLink>

        <NavLink to="/tasks">
          Tasks
        </NavLink>

        {(user?.role === "Admin" ||
          user?.role === "Manager") && (
          <NavLink to="/teams">
            Teams
          </NavLink>
        )}

        {(user?.role === "Admin" ||
          user?.role === "Manager") && (
          <NavLink to="/users">
            Users
          </NavLink>
        )}

        <NavLink to="/notifications">
          Notifications
        </NavLink>

      </nav>

    </aside>
  );
};

export default Sidebar;