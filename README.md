# Employee Management System

A full-stack web application built using Angular, ASP.NET Core, and SQL Server to manage employees, attendance, leaves, tasks, and notifications.

## Features

- Employee and admin login
- Employee and department management
- Leave application and admin approval/rejection
- Email notifications for leave decisions
- Attendance tracking and attendance history
- Employee location tracking with Leaflet and geofencing
- Task assignment and status updates
- Dashboard for employee and administrative information
- Real-time notifications using SignalR

## Technology Stack

- **Frontend:** Angular, TypeScript, SCSS
- **Backend:** ASP.NET Core Web API, C#
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Real-time communication:** SignalR
- **Maps:** Leaflet

## Project Structure

- `web-app/` — Angular frontend
- `EmployeeManagementSystem/` — ASP.NET Core backend

## Run Locally

### Backend

1. Configure your local database connection and required application settings.
2. Open a terminal in the backend project folder.
3. Run:

   `dotnet run`

### Frontend

1. Open a terminal in the `web-app` folder.
2. Install dependencies:

   `npm install`

3. Start the application:

   `npm start`

4. Open `http://localhost:4200` in your browser.

Make sure the frontend API URL matches the address where your backend is running.

## Security

Keep passwords, email credentials, connection strings containing secrets, and other private configuration out of GitHub. Use local configuration files for sensitive settings.

## Author

Developed as a full-stack software engineering project.

## Application Screenshots

### Login Page
![Employee Management System Login](screenshot/Login%20page.png)

### Admin Dashboard
![Admin Dashboard](screenshot/Admin%20dashboard%20with%20statistics%20and%20absent%20summary.png)

### Leave Management and Notifications
![Leave Management](screenshot/Leave%20management%20with%20AcceptReject%20actions%20and%20notifications.png)
