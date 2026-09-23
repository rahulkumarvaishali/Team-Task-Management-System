import { useEffect, useState } from "react";
import { getDashboard } from "../services/taskService";
import { useAuth } from "../context/AuthContext";

const Dashboard = () => {
  const { user } = useAuth();

  const [dashboard, setDashboard] =
    useState(null);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        setLoading(true);

        const data = await getDashboard();

        setDashboard(data);
      } catch (err) {
        console.error(err);

        setError(
          "Unable to load dashboard."
        );
      } finally {
        setLoading(false);
      }
    };

    loadDashboard();
  }, []);

  if (loading) {
    return (
      <div className="page-message">
        Loading dashboard...
      </div>
    );
  }

  if (error) {
    return (
      <div className="error-message">
        {error}
      </div>
    );
  }

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Dashboard</h1>

          <p>
            Welcome back, {user?.fullName}
          </p>
        </div>
      </div>

      <div className="dashboard-grid">
        <DashboardCard
          title="Total Tasks"
          value={dashboard?.totalTasks ?? 0}
        />

        <DashboardCard
          title="To Do"
          value={dashboard?.toDoTasks ?? 0}
        />

        <DashboardCard
          title="In Progress"
          value={
            dashboard?.inProgressTasks ?? 0
          }
        />

        <DashboardCard
          title="Completed"
          value={dashboard?.doneTasks ?? 0}
        />

        <DashboardCard
          title="High Priority"
          value={
            dashboard?.highPriorityTasks ?? 0
          }
        />

        <DashboardCard
          title="Overdue"
          value={
            dashboard?.overdueTasks ?? 0
          }
        />
      </div>
    </div>
  );
};

const DashboardCard = ({ title, value }) => {
  return (
    <div className="dashboard-card">
      <span>{title}</span>

      <h2>{value}</h2>
    </div>
  );
};

export default Dashboard;