import { useState } from "react";
import {
  Link,
  Navigate,
  useNavigate,
} from "react-router-dom";

import { useAuth } from "../context/AuthContext";

const Login = () => {
  const [email, setEmail] =
    useState("");

  const [password, setPassword] =
    useState("");

  const [error, setError] =
    useState("");

  const [loading, setLoading] =
    useState(false);

  const {
    login,
    isAuthenticated,
  } = useAuth();

  const navigate = useNavigate();

  // Already logged in
  if (isAuthenticated) {
    return (
      <Navigate
        to="/dashboard"
        replace
      />
    );
  }

  const handleSubmit = async (e) => {
    e.preventDefault();

    setError("");
    setLoading(true);

    try {
      await login(
        email.trim(),
        password
      );

      navigate(
        "/dashboard",
        {
          replace: true,
        }
      );
    } catch (err) {
      console.error(
        "Login failed:",
        err
      );

      if (
        typeof err.response?.data ===
        "string"
      ) {
        setError(
          err.response.data
        );
      } else if (
        err.response?.data?.message
      ) {
        setError(
          err.response.data.message
        );
      } else {
        setError(
          "Unable to login. Please check your email and password."
        );
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">

      <div className="login-card">

        <div className="login-header">

          <h1>
            Task Management
          </h1>

          <p>
            Sign in to manage your
            tasks and teams
          </p>

        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>

          {/* Email */}

          <div className="form-group">

            <label htmlFor="email">
              Email
            </label>

            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) =>
                setEmail(
                  e.target.value
                )
              }
              placeholder="Enter your email"
              autoComplete="email"
              required
              disabled={loading}
            />

          </div>

          {/* Password */}

          <div className="form-group">

            <label htmlFor="password">
              Password
            </label>

            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) =>
                setPassword(
                  e.target.value
                )
              }
              placeholder="Enter your password"
              autoComplete="current-password"
              required
              disabled={loading}
            />

          </div>

          {/* Login Button */}

          <button
            type="submit"
            className="login-button"
            disabled={loading}
          >
            {loading
              ? "Signing in..."
              : "Sign In"}
          </button>

        </form>

        {/* Register Link */}

        <div className="auth-footer">

          Don't have an account?{" "}

          <Link to="/register">
            Create Account
          </Link>

        </div>

      </div>

    </div>
  );
};

export default Login;