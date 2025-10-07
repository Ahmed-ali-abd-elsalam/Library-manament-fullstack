# 📚 Library Management API

A **real-world Library Management System API** built with **.NET Core**, **Entity Framework Core**, **JWT Authentication**, and **Clean Architecture** principles.
This project demonstrates scalable backend design, secure authentication, and best practices in software architecture.

---

## 🧱 Architecture Overview

This project follows **Clean Architecture**, ensuring clear separation of concerns and maintainability.

```
src/
├── Domain
│   ├── Entities
│   └── Exceptions
│
├── Application
│   ├── DTOs
│   ├── Interfaces
│   └── Services
│
├── Infrastructure
│   ├── DbContext (EF Core)
│   ├── Repository Implementations
│   └── Migrations
│
└── Presentation
    ├── Controllers
    ├── Middlewares
    └── Program.cs
```

* **Domain** → Entities & core business logic
* **Application** → Interfaces, DTOs, use cases, and services
* **Infrastructure** → Database, repository, and external service integrations
* **Presentation** → API layer (Controllers, Filters, Middlewares)

---

## ⚙️ Technologies Used

* **.NET 8 Web API**
* **Entity Framework Core (Code First)**
* **JWT Authentication**
* **PostgreSQL / SQL Server**
* **FluentEmail (SMTP)**
* **Dependency Injection**
* **SOLID Principles**
* **Papercut (Local Email Testing)**

---

## 📘 Entities

### Book

| Field         | Type   | Description         |
| ------------- | ------ | ------------------- |
| Id            | int    | Primary key         |
| Title         | string | Book title          |
| Author        | string | Book author         |
| PublishedYear | int    | Year published      |
| IsAvailable   | bool   | Availability status |

### Member

| Field    | Type     | Description  |
| -------- | -------- | ------------ |
| Id       | int      | Primary key  |
| Name     | string   | Member name  |
| Email    | string   | Member email |
| JoinDate | DateTime | Date joined  |

### BorrowRecord

| Field      | Type      | Description                |
| ---------- | --------- | -------------------------- |
| Id         | int       | Primary key                |
| BorrowDate | DateTime  | When the book was borrowed |
| ReturnDate | DateTime? | When it was returned       |
| BookId     | int       | FK to Book                 |
| MemberId   | int       | FK to Member               |

---

## 🚀 Features

### 📖 Book Management

* `GET /books` → List all books (supports optional pagination/filtering)
* `POST /books` → Add a new book *(Admin only)*
* `PUT /books/{id}` → Update book details *(Admin only)*
* `DELETE /books/{id}` → Delete a book *(Admin only)*

### 👥 Member Management

* `GET /members` → List all members *(Admin only)*
* `POST /members` → Add a new member *(Admin only)*

### 📦 Borrow & Return

* `POST /borrow` → Borrow a book *(requires JWT authentication)*
* `POST /return` → Return a borrowed book

**Rules:**

* Book must be available to borrow.
* Borrowing sets `IsAvailable = false`.
* Returning sets `IsAvailable = true`.
* A member cannot borrow the same book twice simultaneously.

---

## 🔐 Authentication & Authorization

### Endpoints

* `POST /auth/register` → Register a new user (email confirmation required)
* `POST /auth/login` → Login and receive JWT & refresh token
* `POST /auth/refresh` → Refresh JWT using refresh token
* `POST /auth/reset-password` → Reset password via email link

### Security Features

✅ JWT Authentication
✅ Refresh Token System
✅ Multi-Device Login Tracking
✅ Role-Based Access Control (Admin / Member)
✅ Email Confirmation (via FluentEmail + SMTP)
✅ Password Reset
✅ Token Blacklisting (Logout / Token Revocation)

---

## ⚡ Custom Middleware

* **Global Exception Handler**

  * Intercepts unhandled exceptions globally.
  * Returns standardized error responses.
  * Prevents stack trace leaks to frontend.

* **Token Validation Middleware**

  * Ensures token exists in `UserTokens` table.
  * Rejects unauthorized access for revoked tokens.

* **Request Timeout Middleware**

  * Automatically cancels long-running requests.

---

## ✉️ Email Integration

Email features powered by **FluentEmail.SMTP**:

* Email confirmation after registration
* Password reset links
* Works locally with **Papercut** or production SMTP (e.g., Gmail)

---

## 🧩 Bonus Features

* ✅ Pagination & filtering for `/books`
* ✅ Role-based authorization
* ✅ Global Exception Handling
* ✅ Token Table for multiple logins
* ✅ Email confirmation & password reset
* ✅ Timeout middleware
* 🔜 OAuth2 (Planned)

---

## 🧠 Design Principles

* **Clean Architecture**
* **Dependency Injection**
* **Single Responsibility**
* **Open/Closed Principle**
* **Separation of Concerns**

---

## 🧰 Setup Instructions

1. Clone the repository

   ```bash
   git clone https://github.com/your-username/LibraryManagementAPI.git
   cd LibraryManagementAPI
   ```

2. Set up user secrets (for sensitive configs)

   ```bash
   dotnet user-secrets set "DBPassword" "your-db-password"
   dotnet user-secrets set "TokenSecret" "your-jwt-secret"
   dotnet user-secrets set "SmtpUser" "your-email@example.com"
   dotnet user-secrets set "SmtpPass" "your-email-password"
   ```

3. Run database migrations

   ```bash
   dotnet ef database update
   ```

4. Start the project

   ```bash
   dotnet run --launch-profile "LibraryManagementAPI"
   ```

5. Access Swagger UI at

   ```
   https://localhost:5001/swagger
   ```

---

## 🧑‍💻 Author

**Ahmed Ali Abdelsalam**
Backend Developer | .NET • FastAPI • Django
📧 [ahmedali@example.com](mailto:ahmedali@example.com)
🔗 [LinkedIn](https://linkedin.com/in/ahmedaliabdelsalam)
🔗 [GitHub](https://github.com/your-username)

---

## 🏁 License

This project is open-source and available under the [MIT License](LICENSE).
