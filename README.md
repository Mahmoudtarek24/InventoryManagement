# 📦 Inventory Management RESTful API

The system manages supplier purchases, stock storage across multiple warehouses, inter-warehouse transfers, and simulates inventory audits by allowing manual stock adjustments due to surplus, damage, or missing items.

## 📦 Key Features

#### 🔑 Authentication & Authorization
- Secured using JWT with refresh & revoke tokens
- Role information is embedded in tokens for protected route handling
- JWT-based access with roles (admin, Inventory Manager, Supplier, Sales Viewer)

#### 🧑‍💼 User Management
- Admin can register and manage all users and their roles
- Suppliers can register themselves
- Support for viewing user details and updating profile information

#### 🔐 Role-Based Access Control
- Roles: `Admin`, `InventoryManager`, `Supplier`, `SalesViewer`
- Admin has full access; in future updates, Inventory Managers will be restricted to their own warehouse

#### 📦 Product Management
- Products are created by suppliers with base data (name, price, etc.), and extended by the inventory system (barcode, availability)
- Supports full product CRUD operations, bulk creation, and category-based queries

#### 🧑‍💼 Supplier Onboarding & Verification
- Suppliers register and upload tax documents for verification by admins
- Only verified suppliers can receive purchase orders or upload products
- A supplier **cannot perform any action** in the system **until verified by an Admin**.

#### 📥 Purchase Orders
- Inventory managers and Admin can create purchase orders with multiple products and send them to suppliers
- Purchase orders support multiple statuses: `Draft`, `Sent`, `PartiallyReceived`, `Received`, and `Cancelled`

#### 🏢 Warehouse Management
- Multiple warehouses supported, each with individual inventory levels
- Products can be stored in multiple warehouses with separate stock quantities

#### 🔄 Stock Transfers
- Enables transferring products between warehouses
- Automatically updates inventory records and logs the transfer as a stock movement

#### 🛠️ Inventory Adjustments (Stock Audit Simulation)
- Allows manual adjustments to stock quantities (e.g., for lost, damaged, or extra items)
- All adjustments are logged in the stock movement history

#### 📊 Stock Movement Tracking
- Logs every inventory action (purchase, sale, transfer, manual adjustment) with quantity, date, product, and warehouse details
- Query stock movements by product or warehouse

## 🚀 Technical Features

- ✅ Built using clean RESTful principles and SOLID architecture
- ✅ EF Core with Code First approach, using Fluent API for full control over relationships and configurations
- ✅ Soft Delete supported in selected entities like `Category` and `ApplicationUser`, with automatic filtering
- ✅ All key List endpoints support:
  - 🔍 Search
  - 🧩 Dynamic Filtering
  - ↕️ Sorting
  - 📄 Pagination
- ✅ Middleware pipeline includes:
  - 🎯 Global Exception Handling
  - 🖼️ Image Validation
  - 🛡️ Request Lock Middleware (prevents duplicate Purchase Order submission within 1 minute)
- ✅ FluentValidation integrated for all DTOs that require complex validation logic
- ✅ Fully implemented **JWT Authentication** with:
  - Access & Refresh Tokens
  - Token rotation and secure reissue endpoints
- ✅ Role-based access control (RBAC) with `[Authorize(Roles = "...")]`, supporting roles like `Admin`, `InventoryManager`, `Supplier`, and `SalesViewer`
- ✅ All stock-related actions (Purchase, Transfer, Manual Adjustments) are logged via the `StockMovement` table
- ✅ Advanced entity mappings using Fluent API

## 🏗️ Project Structure

```
InventoryManagement/
├── API/
│   └── InventoryManagement/
│       ├── wwwroot/
│       ├── Controllers/
│       ├── Filters/
│       ├── Middleware/
│       ├── Settings/
│       ├── appsettings.json
│       ├── InventoryManagement.http
│       ├── Program.cs
│       └── ServiceCollectionExtensions.cs
│
├── Application/
│   └── Application/
│       ├── Constants/
│       ├── DTO's/
│       ├── Exceptions/
│       ├── Interfaces/
│       ├── Mappings/
│       ├── ResponseDTO's/
│       ├── Services/
│       ├── Settings/
│       ├── validations/
│       └── ServiceCollectionExtensions.cs
│
├── Domain/
│   └── Domain/
│       ├── Entity/
│       ├── Enum/
│       └── Interface/
│
└── Infrastructure/
    └── Infrastructure/
        ├── Configurations/
        ├── Context/
        ├── Enum/
        ├── Identity Models/
        ├── Internal Interfaces/
        ├── Mappings/
        ├── Migrations/
        ├── Repository/
        ├── Seeds/
        ├── Services/
        ├── Settings/
        ├── UnitOfWork/
        ├── Views/
        └── ServiceCollectionExtensions.cs
```

> **⚠️ Note:**  
> To follow clean architecture principles and separation of concerns, the Identity-related models like `ApplicationUser` are **not** placed inside the `Domain` layer.  
> Instead, they are defined in the `Infrastructure` layer only, and the relationship (e.g., between `Supplier` and `ApplicationUser`) is configured using **Fluent API** inside `DbContext`.

## 🚀 Technical Features

#### ✅ Authentication & Authorization
- JWT-based authentication with Refresh Token support
- Role-based authorization using Claims (`Admin`, `Supplier`, `InventoryManager`, `SalesViewer`)
- Centralized user context extraction using custom middleware: [`UserContextMiddleware`](https://github.com/Mahmoudtarek24/InventoryManagement/blob/main/InventoryManagement/Middleware/UserContextMiddleware.cs)
- Relies on [`UserContextService`](https://github.com/Mahmoudtarek24/InventoryManagement/blob/main/Infrastructure/Services/UserContextService.cs) to access `UserId`, `Roles`, `IsSupplier`, `Route` from any layer of the application

#### 🧱 Architecture & Patterns
- Clean Architecture applied with separation into: Domain, Application, Infrastructure, and API layers
- Uses Repository Pattern with Unit of Work for better data handling abstraction
- Dependency Injection is used across the project to manage services and repositories
- Middleware:
  - Global Exception Handling
  - `UserContextMiddleware` to extract and reuse authenticated user info
  - Request locking middleware to prevent duplicate purchase requests in the same minute (admin/inventory manager)

#### 🗄️ Database Features
- Entity Framework Core as ORM
- Support for both **Soft Delete** (e.g., on Suppliers) and **Hard Delete** where needed
- All entity configurations done through **Fluent API** (example: `CategoryConfigurations`)
- Includes SQL **Views**, such as `SupplierProfileView`, for optimized read access
- Proper indexing and Check constraints applied to ensure data integrity (e.g., `builder.HasCheckConstraint("CK_PurchaseOrder_TotalCost_Positive", "[TotalCost] > 0")`)

#### 🧰 Other Features
- Global Exception Handling with centralized logic
- Custom Action Filters:
  - `ValidateImageAttribute` – Validates image file format and size
  - `ValidateModelAttribute` – Automatically validates incoming DTOs using FluentValidation
- DTO Validation with **FluentValidation**
- Full API documentation via **Swagger UI**
- Dynamic configuration for image uploads using `IOptions<ImageSettings>` pattern:
- Example:
  ```json
  "ImageSettings": {
    "MaxFileSize": 5242880,
    "AllowedExtensions": [ ".jpg", ".jpeg", ".png" ]
  }
  ```
- These settings are strongly typed using a custom `ImageSettings` class and injected using the `IOptions<T>` interface, allowing seamless configuration changes per environment (e.g., development vs production)

## 🗄️ Database Documentation

### ⚙️ Database Configuration

- **ORM:** Entity Framework Core
- **Type:** Relational Database
- **Approach:** Code-First
- **Features:**
  - Migrations
  - Relationships (One-to-One, One-to-Many)
  - Views
  - Indexing
  - Soft Delete

### 👁️ Database Views

##### Purpose & Clean Architecture Compliance
This project implements Clean Architecture principles where:

- **Domain Layer:** Contains core business entities like Supplier and cannot see any other layers
- **Infrastructure Layer:** Contains ApplicationUser (Identity tables) and can see the Domain Layer

##### The Problem:
- Cannot add ApplicationUser as a Navigation Property in Supplier because the Domain Layer cannot see the Infrastructure Layer
- However, we need access to user information related to the supplier

##### The Solution:
Database Views were created to retrieve complete supplier and related user information without violating Clean Architecture principles.

##### Benefits:
- ✅ Maintains Clean Architecture principles
- ✅ Does not violate dependency rules
- ✅ Retrieves complete data in a single query
- ✅ Improves performance

### 🔗 Entity Relationships Table

| Parent Entity | Child Entity | Relationship Type | Foreign Key | Description |
|---------------|---------------|------------------|-------------|-------------|
| Category | Product | One-to-Many | CategoryId | One category can have multiple products |
| Supplier | Product | One-to-Many | SupplierId | One supplier can provide multiple products |
| ApplicationUser | Supplier | One-to-One | userid | One user is linked to one supplier |
| Supplier | PurchaseOrder | One-to-Many | SupplierId | One supplier can create multiple purchase orders |
| Warehouse | PurchaseOrder | One-to-Many | WarehouseId | One warehouse can receive multiple purchase orders |
| PurchaseOrder | PurchaseOrderItem | One-to-Many | PurchaseOrderId | One purchase order contains multiple items |
| Product | PurchaseOrderItem | One-to-Many | ProductId | One product can be in multiple purchase order items |
| Warehouse | Inventory | One-to-Many | WarehouseId | One warehouse contains inventory of multiple products |
| Product | Inventory | One-to-Many | ProductId | One product can have inventory in multiple warehouses |
| Product | StockMovement | One-to-Many | ProductId | One product can have multiple stock movements |
| Warehouse | StockMovement (Source) | One-to-Many | SourceWarehouseId | One warehouse can be source of multiple stock movements |
| Warehouse | StockMovement (Destination) | One-to-Many | DestinationWarehouseId | One warehouse can be destination of multiple stock movements |

## 🚀 How to Use the System

```http
POST /api/auth/login
{
  "email": "admin@Inventory.com",
  "password": "P@ssword123"
}
```
