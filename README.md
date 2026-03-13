# TimeSheet Management System

A full-stack web application designed for tracking employee work hours, managing projects, and generating detailed reports. This project was built with a focus on **clean code**, **scalability**, and **user experience**.

## Key Features

* **Interactive Calendar**: Monthly and daily views for logging activities.
* **Team Management**: Admin tools to assign members to projects and designate Project Leads.
* **Business Rule Validation**:
* Maximum **8 hours** of standard time per day.
* Maximum **4 hours** of overtime per day.
* Total daily limit of **12 hours**.


* **Reporting**: Export work logs to **PDF** and **Excel** formats.
* **Dynamic UI**: Full **Dark Mode** support and responsive design.
* **Role-Based Access**: Specialized views for Administrators and regular Members.

---

## Architecture & Tech Stack

### Backend (.NET Core)

The backend follows the **Onion Architecture** (Clean Architecture) pattern to ensure a separation of concerns and high testability.

* **Domain Layer**: Core entities (Member, Project, Activity, Client) and Enums.
* **Application Layer**: Data Transfer Objects (DTOs), Interfaces, and Business logic.
* **Infrastructure Layer**: Data persistence using **Entity Framework Core** and **PostgreSQL**.
* **Web API Layer**: RESTful controllers with JWT Authentication.

### Frontend (React & TypeScript)

A modern, type-safe frontend built for performance.

* **React (Functional Components & Hooks)**
* **TypeScript** for robust type checking.
* **Axios** for API communication.
* **CSS3** with variable-based theming (Light/Dark mode).
* **Libraries**: `jsPDF` for PDF generation, `XLSX` for Excel exports.

---

## Database Schema

The system manages relationships between:

* **Clients** and **Projects** (One-to-Many).
* **Members** and **Projects** (Many-to-Many via `ProjectMember` junction table).
* **Members** and **Activities** (One-to-Many).

---

## Business Logic: Daily Work Hour Limits

To comply with labor regulations and company policies, the system implements a strict validation gate during the activity saving process:

---

## Getting Started

### Prerequisites

* .NET 10.0+ SDK
* Node.js & npm
* PostgreSQL instance

### Installation

1. **Clone the repository**
2. **Backend Setup**:
* Update `appsettings.json` with your PostgreSQL connection string.
* Run `dotnet ef database update` to apply migrations.
* Run `dotnet run`.


3. **Frontend Setup**:
* Run `npm install`.
* Run `npm start`.
