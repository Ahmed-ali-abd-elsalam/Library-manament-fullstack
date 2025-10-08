# 📚 Library Management API

A small but real-world **Library Management API** built with **ASP.NET Core**, **Entity Framework Core**, **JWT Authentication**, and **Clean Architecture** principles.
This project demonstrates key backend concepts like **token-based authentication**, **email confirmation**, **password resets**, **Redis caching**, and **clean layering**.

---

## 📑 Table of Contents

1. [🏗️ Architecture Overview](#️-architecture-overview)
2. [⚙️ Prerequisites](#️-prerequisites)
3. [📘 Entities](#-entities)
4. [🚀 Features Overview](#-features-overview)
5. [🔐 Authentication (JWT)](#-authentication-jwt)
6. [🚪 Logout & Token Revocation](#-logout--token-revocation)
7. [♻️ Refresh Token Flow](#-refresh-token-flow)
8. [📧 Email Validation](#-email-validation)
9. [🔑 Password Reset Flow](#-password-reset-flow)
10. [🧰 Development vs Production Email Sending](#-development-vs-production-email-sending)
11. [⚙️ Global Exception Handling](#️-global-exception-handling)
12. [🧩 OpenAPI Endpoints Summary](#-openapi-endpoints-summary)
13. [🧠 Summary](#-summary)

---

## 🏗️ Architecture Overview

The solution follows **Clean Architecture** with four main layers:

### **1. Domain**

* Contains **Entities** (`Book`, `Member`, `BorrowRecord`) and business rules.

### **2. Application**

* Contains **use cases**, **DTOs**, and **interfaces** for services and repositories.
* Example: `IBookService`, `IMemberService`, `IBorrowService`.

### **3. Infrastructure**

* Contains EF Core **DbContext**, **repository implementations**, and **database migrations**.
* Handles all data persistence and external integrations (e.g., Redis, SMTP).

### **4. Presentation**

* ASP.NET Core **Controllers** for RESTful APIs.
* Handles request/response mapping, middleware pipeline, and authentication.

---

## ⚙️ Prerequisites

Before running the project locally, ensure the following services are installed and configured:

| Tool              | Purpose                                                | Installation Notes                                                                     |
| ----------------- | ------------------------------------------------------ | -------------------------------------------------------------------------------------- |
| **Redis**         | Token storage, blacklisting, and refresh token caching | Install locally or use Docker (`docker run -d -p 6379:6379 redis`)                     |
| **PaperCut SMTP** | Email testing in development (no real emails sent)     | [Download PaperCut SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP/releases) |
| **Gmail SMTP**    | Production email sending                               | Configure credentials under `appsettings.Production.json` or User Secrets              |

---

## 📘 Entities

| Entity           | Fields                                        | Description                        |
| ---------------- | --------------------------------------------- | ---------------------------------- |
| **Book**         | Id, Title, Author, PublishedYear, IsAvailable | Represents a library book          |
| **Member**       | Id, Name, Email, JoinDate                     | Represents a library member        |
| **BorrowRecord** | Id, BorrowDate, ReturnDate, BookId, MemberId  | Represents a borrowing transaction |

---

## 🚀 Features Overview

### 📚 **Book Management**

* `GET /api/books` → List all books (paginated)
* `POST /api/books/add` → Add a new book (**Admin only**)
* `PUT /api/books/{BookId}` → Update book details
* `DELETE /api/books/{BookId}` → Delete book

### 👥 **Member Management**

* `GET /api/members` → List all members
* `POST /api/members` → Add a new member

### 🔄 **Borrow / Return**

* `POST /api/borrow/{BookId}` → Borrow a book (**Member only**)
* `POST /api/return/{BookId}` → Return a borrowed book (**Member only**)

---

## 🔐 Authentication (JWT)

* **Registration** → `POST /api/Auth/register`
* **Login** → `POST /api/Auth/login`
* **Logout** → `DELETE /api/Auth/logout`
* **Refresh Token** → `POST /api/Auth/refresh`

Protected routes require `[Authorize]` attributes, ensuring only authenticated users access restricted endpoints.

---

## 🚪 Logout & Token Revocation

Redis tracks all active JWTs by user and login source.
Middleware checks every request for valid tokens.

* Missing token in Redis →

  ```json
  { "error": "Invalid Token" }
  ```
* Token removed on logout or expiry.

✅ Supports **multi-device login** and **immediate revocation**.

---

## ♻️ Refresh Token Flow

| Event          | Access Token    | Refresh Token | Redis Entry | Result   |
| -------------- | --------------- | ------------- | ----------- | -------- |
| Login          | Issued          | Issued        | Stored      | ✅        |
| Access expires | Invalid         | Valid         | Exists      | 🔄 Renew |
| Refresh used   | New pair issued | Old removed   | Updated     | ✅        |
| Logout         | Revoked         | Revoked       | Removed     | ❌        |
| Reuse attempt  | Invalid         | Invalid       | Missing     | ❌ 401    |

---

## 📧 Email Validation

New users receive an email with a **confirmation link**:
`/api/Auth/confirm-email?email=<user>&token=<token>`

If the token matches → account becomes **verified**.

---

## 🔑 Password Reset Flow

* Start → `GET /api/Auth/forget-password-start?email=<email>`
* Reset → `PUT /api/Auth/forget-password?token=<token>` with:

  ```json
  {
    "email": "user@example.com",
    "newPassword": "MyNewPass123",
    "confirmNewPassword": "MyNewPass123"
  }
  ```

If the token is valid → password is updated.

---

## 🧰 Development vs Production Email Sending

| Environment     | SMTP       | Description                       |
| --------------- | ---------- | --------------------------------- |
| **Development** | PaperCut   | Captures emails locally           |
| **Production**  | Gmail SMTP | Sends real emails via FluentEmail |

---

## ⚙️ Global Exception Handling

Middleware returns standardized error responses instead of stack traces:

```json
{
  "statusCode": 400,
  "error": "Invalid Request",
  "message": "The provided data is invalid."
}
```

✅ No try/catch needed in controllers
✅ Centralized, safe error reporting

---

## 🧩 OpenAPI Endpoints Summary

### **Auth**

| Method   | Endpoint                          | Description             |
| -------- | --------------------------------- | ----------------------- |
| `POST`   | `/api/Auth/register`              | Register new member     |
| `POST`   | `/api/Auth/login`                 | Login and get JWT       |
| `DELETE` | `/api/Auth/logout`                | Logout and revoke token |
| `POST`   | `/api/Auth/refresh`               | Refresh access token    |
| `GET`    | `/api/Auth/confirm-email`         | Confirm user email      |
| `GET`    | `/api/Auth/forget-password-start` | Start password reset    |
| `PUT`    | `/api/Auth/forget-password`       | Complete password reset |

### **Books**

| Method   | Endpoint              | Role   | Description          |
| -------- | --------------------- | ------ | -------------------- |
| `GET`    | `/api/books`          | Public | Get all books        |
| `POST`   | `/api/books/add`      | Admin  | Add a new book       |
| `PUT`    | `/api/books/{BookId}` | Admin  | Update existing book |
| `DELETE` | `/api/books/{BookId}` | Admin  |                      |
