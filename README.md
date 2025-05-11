# JobApplicationTracker
A personal job tracking web app built with ASP.NET Core MVC. Easily manage and monitor all your job applications in one place with search, statistics, export, and more.
---

## Tech Stack

- **ASP.NET Core MVC** (.NET 9)
- **Entity Framework Core** (SQL Server)
- **ASP.NET Identity** (User Authentication)
- **Bootstrap 5** (Responsive UI)
- **Chart.js** (Data Visualization) 
- **CsvHelper** (CSV Export/Import)
---

## Features

### Core Features
- Add, edit, delete job applications
- Track company, title, status, date applied, and notes
- Secure login system with user-specific dashboards

### Search & Filter
- Filter by:
  - Company
  - Application Status
  - Date Applied
- Full-text search by job title and notes

## Features coming soon

### Dashboard (with Chart.js)
- Application stats by status
- Applications per month
- Offer vs. Rejection ratios

### Import / Export
- Export applications to `.csv`
- Import from `.csv` 
---

## Screenshots
![image](https://github.com/user-attachments/assets/a3f5320d-9849-43a0-9e2b-52255495ac96)
![image](https://github.com/user-attachments/assets/6ff291cb-0903-4572-8313-d407a94ca62f)
![image](https://github.com/user-attachments/assets/f721443f-5ab7-4e71-81bb-de5d8fbbf8cc)

---

## Setup Instructions

### 1. Clone the Repo

```bash
git clone https://github.com/your-username/job-tracker.git
cd job-tracker
```
### 2. Setup the database
Make sure you have SQL Server (or SQLite) installed.
```bash
dotnet ef database update
```

### 3. Run the App
```bash
dotnet run
```
Visit the app at: https://localhost:5001
