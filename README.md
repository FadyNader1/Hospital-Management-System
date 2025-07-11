# Hospital Management System

A comprehensive web-based Hospital Management System built with ASP.NET Core (.NET 8), Entity Framework Core, and SQL Server. This system streamlines hospital operations, including patient management, doctor management, appointments, medical reports, and user authentication.

---

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- **User Authentication & Authorization** (JWT, ASP.NET Identity)
- **Role Management** (Admin, Doctor, Patient, Staff, User)
- **Patient Management** (CRUD operations)
- **Doctor Management** (CRUD operations, availability, appointments)
- **Appointment Scheduling** (Create, update, approve, reject, delete)
- **Medical Reports** (Add, view, retrieve by ID)
- **Dashboard Statistics** (Total patients, doctors, appointments, reports)
- **Error Handling** (Validation and API errors)
- **Swagger Integration** (API documentation and testing)

---

## Technologies Used

- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server
- AutoMapper
- JWT Authentication
- ASP.NET Identity
- Swagger (OpenAPI)
- C#

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Visual Studio 2022 or later

### Installation

1. **Clone the repository:**
   git clone https://github.com/yourusername/Hospital-Management-System.git cd Hospital-Management-System

2. **Configure the database connection:**
- Update the connection string in `appsettings.json` under `"ConnectionStrings": { "DC": "..." }`.

3. **Apply migrations:**
   dotnet ef database update

5. **Run the application:**
   dotnet run
The API will be available at `https://localhost:5001` (or as configured).

---

## API Documentation

### Authentication

- **POST** `/api/Account/Register`  
Register a new user.

- **POST** `/api/Account/Login`  
Login and receive JWT token.

### Doctor Management

- **POST** `/api/Doctor/AddDoctor` 
Add a new doctor. *(Admin only)*

- **GET** `/api/Doctor/GetAllDoctors?SearchBySpecialization={specialization}` 
Get all doctors, filter by specialization.

- **GET** `/api/Doctor/GetDoctorById/{id}` 
Get doctor details by ID.

- **PUT** `/api/Doctor/UpdateDoctor/{id}` 
Update doctor details. *(Admin only)*

- **DELETE** `/api/Doctor/DeleteDoctor/{id}` 
Delete a doctor. *(Admin only)*

- **GET** `/api/Doctor/DoctorAppointments/{id}` 
Get all appointments for a doctor. *(Doctor only)*

### Patient Management

- **POST** `/api/Patient/AddPatient` 
Add a new patient.

- **GET** `/api/Patient/GetAllPatients` 
Get all patients. *(Admin, Staff)*

- **GET** `/api/Patient/GetPatientById?id={id}` 
Get patient details by ID.

- **PUT** `/api/Patient/UpdatePatient?id={id}`  
Update patient details. *(Admin, Staff)*

### Appointment Management

- **POST** `/api/Appointment/AddAppointment`  
Add a new appointment. *(Patient only)*

- **GET** `/api/Appointment/GetAllAppointments`  
Get all appointments. *(Patient only)*

- **GET** `/api/Appointment/GetAppointmentById/{id}` 
Get appointment details by ID.

- **PUT** `/api/Appointment/UpdateAppointment/{id}` 
Update appointment details. *(Patient only)*

- **DELETE** `/api/Appointment/DeleteAppointment/{id}` 
Delete an appointment. *(Patient only)*

- **POST** `/api/Appointment/ApproveAppointment/{id}` 
Approve an appointment. *(Doctor, Staff)*

- **POST** `/api/Appointment/RejectAppointment/{id}`  
Reject an appointment. *(Doctor, Staff)*

### Medical Report Management

- **POST** `/api/MedicalReport/AddMedicalReport`  
Add a new medical report. *(Doctor only)*

- **GET** `/api/MedicalReport/GetAllMedicalReport` 
Get all medical reports. *(Doctor, Staff)*

- **GET** `/api/MedicalReport/GetMedicalReportById/{id}` 
Get medical report details by ID.

### Dashboard

- **GET** `/api/Dashboard/stats` 
Get statistics (total patients, doctors, appointments, medical reports). *(Admin only)*

---

## Project Structure

- `Controllers/` – API controllers
- `Core/Entities/` – Domain models
- `DTOs/` – Data transfer objects
- `Repository/Contexts/` – EF Core DbContext
- `Repository/Specifications/` – Query specifications
- `Helper/` – Utility classes (e.g., seeding, image handling)
- `Extensions/` – Service registration extensions
- `Middlewares/` – Custom middlewares
- `Services/` – Business logic services

---

## Contributing

Contributions are welcome! 
Please fork the repository, create a feature branch, and submit a pull request.

1. Fork the repo
2. Create your feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -am 'Add new feature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a pull request

---

## License

This project is licensed under the MIT License.

---

## Contact

For questions or support, please open an issue or contact the repository owner.
