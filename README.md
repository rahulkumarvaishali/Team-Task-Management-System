# Team Task Management System

A full-stack role-based **Team Task Management System** built using **ASP.NET Core Web API, React, SQL Server, and Entity Framework Core**.

The application allows organizations to manage teams, assign tasks, track task progress, communicate through comments, receive notifications, and monitor task activity using role-based access control.

---

## Features

### Authentication & Authorization

- User Registration and Login
- JWT-based Authentication
- Password Hashing
- Role-Based Access Control (RBAC)
- Protected Backend APIs
- Protected Frontend Routes

### User Roles

The application supports three roles:

#### Admin
- Manage users
- Change user roles
- Create and manage teams
- Assign managers to teams
- Add/remove team members
- Create and assign tasks
- Update and delete tasks
- View all tasks
- View dashboard statistics
- View task activity history
- Manage comments

#### Manager
- View managed teams
- Manage members of assigned teams
- Create tasks for managed teams
- Assign tasks to team members
- Update tasks
- Update task status
- View team tasks
- Add comments
- Receive notifications

#### User
- View assigned tasks
- Update assigned task status
- Add comments
- Delete own comments
- Receive notifications
- View personal dashboard statistics

---

## Task Management

Tasks support:

- Title
- Description
- Priority
- Deadline
- Team assignment
- User assignment
- Task status
- Comments
- Notifications
- Activity history

### Task Status

- To Do
- In Progress
- Done

A completed task cannot be moved back to To Do or In Progress.

### Priority

- Low
- Medium
- High

### Overdue Tasks

A task is considered overdue when:

- Its deadline has passed
- Its status is not `Done`

Overdue task information is displayed in the application and dashboard.

---

## Team Management

Admins can:

- Create teams
- Assign a Manager to a team
- Update teams
- Delete teams
- Manage team members

Managers can manage members of teams assigned to them.

Task assignment is restricted so that Managers cannot assign tasks to users outside their managed team.

---

## Comments

Users with access to a task can communicate using task comments.

Features include:

- Add comments
- View task comments
- Delete own comments
- Admin can delete comments
- Unauthorized users cannot access comments for restricted tasks

---

## Notifications

The system provides in-application notifications.

Notifications are generated when:

- A task is assigned to a user
- A task is reassigned
- Task status changes

Users can:

- View notifications
- Mark a notification as read
- Mark all notifications as read

---

## Task Activity History

The system maintains an audit-style activity history for important task actions.

Tracked activities include:

- Task Created
- Task Assigned/Reassigned
- Priority Changed
- Deadline Changed
- Status Changed

Activity history records:

- User name
- User role
- Action
- Action details
- Date and time

Task Activity History is available only to **Admin** users.

---

## Dashboard

Role-based dashboards provide task statistics.

Dashboard information includes:

- Total Tasks
- To Do Tasks
- In Progress Tasks
- Done Tasks
- Low Priority Tasks
- Medium Priority Tasks
- High Priority Tasks
- Overdue Tasks

Dashboard data is automatically restricted according to the logged-in user's role.

---

## Task Filtering

Tasks can be filtered by:

- Status
- Priority
- Deadline

---

# Technology Stack

## Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server
- JWT Authentication
- ASP.NET Core Identity Password Hasher
- Swagger / OpenAPI

## Frontend

- React
- Vite
- JavaScript
- Axios
- React Router
- CSS

## Testing

- xUnit
- Entity Framework Core InMemory Database

## Development Tools

- Visual Studio
- Visual Studio Code
- SQL Server
- Swagger
- Git
- GitHub

---

# Architecture

The project follows a layered architecture.

```text
React Frontend
      |
      | HTTP / Axios
      v
ASP.NET Core Web API
      |
      v
Controllers
      |
      v
Service Layer
      |
      v
Entity Framework Core
      |
      v
SQL Server
```

The backend separates responsibilities using:

```text
Controllers
DTOs
Services
Interfaces
Models
Data
Middleware
Helpers
```

---

# Project Structure

```text
TaskManagement/
|
|-- TaskManagement.API/
|   |
|   |-- Controllers/
|   |-- Data/
|   |-- DTOs/
|   |-- Helpers/
|   |-- Interfaces/
|   |-- Middleware/
|   |-- Models/
|   |-- Services/
|   |-- Migrations/
|   |-- Program.cs
|   `-- appsettings.json
|
|-- TaskManagement.Tests/
|   |
|   |-- Controllers/
|   |-- Helpers/
|   `-- Services/
|
|-- task-management-client/
|   |
|   |-- src/
|   |   |-- api/
|   |   |-- components/
|   |   |-- context/
|   |   |-- layouts/
|   |   |-- pages/
|   |   `-- services/
|   |
|   |-- package.json
|   `-- vite.config.js
|
|-- .gitignore
`-- README.md
```

---

# Database

The application uses **SQL Server** with **Entity Framework Core**.

Main entities include:

- User
- Team
- TaskItem
- Comment
- Notification
- TaskActivity

Entity Framework Core migrations are used for database schema management.

---

# Getting Started

## Prerequisites

Install:

- .NET SDK
- SQL Server
- Node.js
- npm
- Git

Visual Studio or Visual Studio Code can be used for development.

---

# Backend Setup

### 1. Clone the Repository

```bash
git clone <YOUR-GITHUB-REPOSITORY-URL>
```

Navigate to the backend project.

```bash
cd TaskManagement.API
```

### 2. Configure Database

Update the connection string in `appsettings.json` according to your local SQL Server configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING"
  }
}
```

Do not commit production passwords or other sensitive credentials.

### 3. Configure JWT

Configure JWT settings in `appsettings.json`.

Example:

```json
{
  "Jwt": {
    "Key": "YOUR_DEVELOPMENT_JWT_SECRET_KEY",
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient",
    "ExpiryMinutes": 60
  }
}
```

For production environments, secrets should be stored securely rather than committed to source control.

### 4. Apply Database Migrations

Using Visual Studio Package Manager Console:

```powershell
Update-Database
```

Or using the .NET CLI:

```bash
dotnet ef database update
```

### 5. Run Backend

```bash
dotnet run
```

The API will start using the URL configured in the ASP.NET Core launch settings.

---

# Swagger API Documentation

When the backend is running, open the Swagger endpoint shown by the application.

Typically:

```text
https://localhost:<PORT>/swagger
```

Swagger can be used to test:

- Authentication
- Users
- Teams
- Tasks
- Comments
- Notifications
- Dashboard
- Task Activity

For protected APIs, authenticate first and provide the JWT using Swagger's **Authorize** option.

---

# Frontend Setup

Navigate to the React application:

```bash
cd task-management-client
```

Install dependencies:

```bash
npm install
```

Create/configure the frontend environment file:

```env
VITE_API_BASE_URL=https://localhost:<API_PORT>/api
```

Start the React development server:

```bash
npm run dev
```

The application normally runs at:

```text
http://localhost:5173
```

---

# Authentication Flow

```text
User Login
    |
    v
POST /api/Auth/login
    |
    v
Validate Credentials
    |
    v
Generate JWT
    |
    v
React stores token
    |
    v
Axios sends JWT
    |
    v
Protected API
```

---

# Role-Based Access Control

| Feature | Admin | Manager | User |
|---|:---:|:---:|:---:|
| View Dashboard | Yes | Yes | Yes |
| View Assigned Tasks | Yes | Yes | Yes |
| Create Task | Yes | Yes | No |
| Edit Task | Yes | Yes | No |
| Delete Task | Yes | Yes | No |
| Update Task Status | Yes | Yes | Yes |
| Create Team | Yes | No | No |
| Manage Team Members | Yes | Yes* | No |
| Manage User Roles | Yes | No | No |
| Add Comments | Yes | Yes | Yes |
| View Notifications | Yes | Yes | Yes |
| View Activity History | Yes | No | No |

`*` Managers can manage members only within teams they are authorized to manage.

---

# API Overview

## Authentication

```text
POST /api/Auth/register
POST /api/Auth/login
```

## Users

```text
GET   /api/Users
GET   /api/Users/{id}
PATCH /api/Users/{id}/role
```

## Teams

```text
GET    /api/Teams
GET    /api/Teams/{id}
POST   /api/Teams
PUT    /api/Teams/{id}
DELETE /api/Teams/{id}

POST   /api/Teams/{teamId}/members/{userId}
DELETE /api/Teams/{teamId}/members/{userId}
```

## Tasks

```text
GET    /api/Tasks
GET    /api/Tasks/{id}
POST   /api/Tasks
PUT    /api/Tasks/{id}
PATCH  /api/Tasks/{id}/status
DELETE /api/Tasks/{id}

GET /api/Tasks/dashboard
GET /api/Tasks/{id}/activity
```

## Comments

```text
GET    /api/tasks/{taskId}/comments
POST   /api/tasks/{taskId}/comments
DELETE /api/comments/{commentId}
```

## Notifications

```text
GET   /api/Notifications
PATCH /api/Notifications/{id}/read
PATCH /api/Notifications/read-all
```

---

# Testing

The backend contains automated tests using **xUnit**.

Tests cover important functionality such as:

- Authentication
- Role-based authorization
- Task creation
- Manager team restrictions
- Task assignment restrictions
- Task status updates
- Completed-task restrictions
- Team management
- Comment permissions
- Activity history

Run tests using Visual Studio Test Explorer or:

```bash
dotnet test
```

Example expected result:

```text
Passed: All configured tests
Failed: 0
```

---

# Security

The application includes:

- JWT authentication
- Password hashing
- Role-based authorization
- Protected API endpoints
- Team-based Manager authorization
- Task ownership/access validation
- Input validation
- Restricted activity-history access
- Token expiration

Sensitive production credentials should never be committed to the repository.

---

# Validation

The application validates important business rules, including:

- Required task title
- Valid task status
- Valid task priority
- Task deadline validation
- Required team Manager
- Team membership validation
- Task assignment restrictions
- Comment validation
- Role-based permissions

---

# Key Business Rules

1. Public registration creates a normal User account.
2. Admin controls role changes.
3. Teams require an assigned Manager.
4. Managers can manage only authorized teams.
5. Managers can assign tasks only within their team.
6. Users can access and update only authorized/assigned tasks.
7. A `Done` task cannot return to `ToDo` or `InProgress`.
8. Tasks past their deadline and not completed are considered overdue.
9. Activity history is available only to Admin users.

---

# Future Improvements

Possible future enhancements:

- Email notifications
- Real-time notifications using SignalR
- File attachments
- Advanced search
- Pagination
- Refresh tokens
- Docker deployment
- CI/CD pipeline
- Cloud deployment
- Extended integration testing

---

# Author

**Rahul Kumar**

.NET Backend Developer

Technologies:

`C#` `ASP.NET Core` `Web API` `SQL Server` `Entity Framework Core` `React` `JavaScript`

---

# License

This project was developed as a technical assessment project.