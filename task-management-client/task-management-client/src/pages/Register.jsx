import { useState } from "react";
import {
  Link,
  Navigate,
  useNavigate,
} from "react-router-dom";

import { register } from "../services/authService";
import { useAuth } from "../context/AuthContext";

const Register = () => {
  const navigate = useNavigate();

  const { isAuthenticated } = useAuth();

  const [form, setForm] = useState({
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const [loading, setLoading] =
    useState(false);

  const [error, setError] =
    useState("");

  const [success, setSuccess] =
    useState("");

  // Already logged in
  if (isAuthenticated) {
    return (
      <Navigate
        to="/dashboard"
        replace
      />
    );
  }

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
    setSuccess("");

    if (!form.fullName.trim()) {
      setError(
        "Full name is required."
      );
      return;
    }

    if (!form.email.trim()) {
      setError(
        "Email is required."
      );
      return;
    }

    if (!form.password) {
      setError(
        "Password is required."
      );
      return;
    }

    if (
      form.password.length < 6
    ) {
      setError(
        "Password must be at least 6 characters."
      );
      return;
    }

    if (
      form.password !==
      form.confirmPassword
    ) {
      setError(
        "Passwords do not match."
      );
      return;
    }

    try {
      setLoading(true);

      await register(
        form.fullName.trim(),
        form.email.trim(),
        form.password
      );

      setSuccess(
        "Account created successfully. Redirecting to login..."
      );

      setTimeout(() => {
        navigate(
          "/login",
          {
            replace: true,
          }
        );
      }, 1000);
    } catch (err) {
      console.error(
        "Registration failed:",
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
      } else if (
        err.response?.data?.title
      ) {
        setError(
          err.response.data.title
        );
      } else {
        setError(
          "Registration failed. Please try again."
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
            TaskFlow
          </h1>

          <h2>
            Create Account
          </h2>

          <p>
            Register to start managing
            your tasks
          </p>

        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        {success && (
          <div className="success-message">
            {success}
          </div>
        )}

        <form onSubmit={handleSubmit}>

          {/* Full Name */}

          <div className="form-group">

            <label htmlFor="fullName">
              Full Name
            </label>

            <input
              id="fullName"
              type="text"
              name="fullName"
              value={form.fullName}
              onChange={handleChange}
              placeholder="Enter your full name"
              autoComplete="name"
              required
              disabled={loading}
            />

          </div>

          {/* Email */}

          <div className="form-group">

            <label htmlFor="email">
              Email
            </label>

            <input
              id="email"
              type="email"
              name="email"
              value={form.email}
              onChange={handleChange}
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
              name="password"
              value={form.password}
              onChange={handleChange}
              placeholder="Enter password"
              autoComplete="new-password"
              required
              disabled={loading}
            />

          </div>

          {/* Confirm Password */}

          <div className="form-group">

            <label htmlFor="confirmPassword">
              Confirm Password
            </label>

            <input
              id="confirmPassword"
              type="password"
              name="confirmPassword"
              value={
                form.confirmPassword
              }
              onChange={handleChange}
              placeholder="Confirm password"
              autoComplete="new-password"
              required
              disabled={loading}
            />

          </div>

          {/* Create Account */}

          <button
            type="submit"
            className="login-button"
            disabled={loading}
          >
            {loading
              ? "Creating Account..."
              : "Create Account"}
          </button>

        </form>

        <div className="auth-footer">

          Already have an account?{" "}

          <Link to="/login">
            Sign In
          </Link>

        </div>

      </div>

    </div>
  );
};

export default Register;