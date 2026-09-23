import {
  Navigate,
  Route,
  Routes,
} from "react-router-dom";

import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Tasks from "./pages/Tasks";
import TaskDetails from "./pages/TaskDetails";
import Teams from "./pages/Teams";

import ProtectedRoute from "./components/ProtectedRoute";
import DashboardLayout from "./layouts/DashboardLayout";
import Users from "./pages/Users";
import Notifications from "./pages/Notifications";
import Register from "./pages/Register";

function App() {
  return (
    <Routes>
      <Route
        path="/login"
        element={<Login />}
      />
      <Route
        path="/register"
        element={<Register />}
      />

      <Route
        element={
          <ProtectedRoute>
            <DashboardLayout />
          </ProtectedRoute>
        }
      >
        <Route
          path="/dashboard"
          element={<Dashboard />}
        />

        <Route
          path="/tasks"
          element={<Tasks />}
        />

        <Route
          path="/tasks/:id"
          element={<TaskDetails />}
        />

        <Route
          path="/teams"
          element={
            <ProtectedRoute
              allowedRoles={[
                "Admin",
                "Manager",
              ]}
            >
              <Teams />
            </ProtectedRoute>
          }
        />

        <Route
          path="/users"
          element={
            <ProtectedRoute
              allowedRoles={[
                "Admin",
                "Manager",
              ]}
            >
              <Users />
            </ProtectedRoute>
          }
        />
        <Route
          path="/notifications"
          element={<Notifications />}
        />

      </Route>

      <Route
        path="/"
        element={
          <Navigate
            to="/dashboard"
            replace
          />
        }
      />

      <Route
        path="*"
        element={
          <Navigate
            to="/dashboard"
            replace
          />
        }
      />
    </Routes>
  );
}

export default App;