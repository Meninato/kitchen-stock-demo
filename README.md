# KitchenStock - Smart Kitchen Inventory Management

A micro-SaaS solution for managing kitchen inventory

---

## 📖 About This Project
This is a demonstration project showcasing my development skills and how to leverage good architectural patterns to create a solid web application.  
While not fully complete, it covers a strong foundation and essential components for implementing a robust Monolithic application.

---

## ✅ Development Status

**Completed Features**
- ✅ Kitchens - Complete
- ⏳ Ingredients - Almost complete

**Not Yet Implemented**
- ⬜ Recipes  
- ⬜ Stock Entries  
- ⬜ Suppliers  
- ⬜ Users Management  

---

## 🏗️ Architecture

| Component     | Technology                                  |
|---------------|---------------------------------------------|
| **Backend**   | .NET 9 Web API with Entity Framework Core    |
| **Frontend**  | Next.js 15 with TypeScript                   |
| **Database**  | PostgreSQL                                  |
| **Auth**      | JWT with HttpOnly refresh tokens             |

---

## 🛠️ Tech Stack

### Backend
- [FluentResults](https://github.com/altmann/FluentResults) – Result Pattern implementation  
- [MediatR](https://github.com/jbogard/MediatR) – CQRS pattern  
- Services & Repository Pattern – Clean architecture  
- Entity Framework Core – Code-First approach  

### Frontend
- React with TypeScript  
- TailwindCSS – Styling  
- [Shadcn/ui](https://ui.shadcn.com/) – UI components  
- [React Query (TanStack Query)](https://tanstack.com/query/latest) – Server state management  
- [Zustand](https://zustand-demo.pmnd.rs/) – Client state management  

---

## 🌟 Highlight Component

**API Client** – A robust wrapper that handles:  
- 401 authentication errors  
- Automatic token refresh  
- Race condition prevention  

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)  
- [Docker & Docker Compose](https://www.docker.com/)  

---

### Backend Setup

#### 1. Start Docker Services
```bash
cd KitchenStock.Monolith
docker-compose up
```
This will start:
- PostgreSQL database  
- HashiCorp Vault  

---

#### 2. Configure Vault

**Initial Setup (First Time Only)**  
After `docker-compose up`, open a new terminal and run:

```bash
docker exec -it kitchen-vault vault operator init
```

📝 Important: Save the 5 unseal keys and root token that are generated.
You'll need them every time you restart Docker.

If you encounter permission issues:
```bash
# Access the vault container
docker exec -it kitchen-vault sh

# Fix permissions
cd vault
chmod -R 777 data

# Exit and re-run init command
exit
docker exec -it kitchen-vault vault operator init
```
---

#### 3. Unseal and Access Vault

Open browser: http://localhost:8200  
Use your unseal keys and root token to access


