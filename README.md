# Zento API - Inventory Module

A .NET 8 Web API for providers management built with **Vertical Slice Architecture** and clean architecture principles.

## Architecture

This project follows **Vertical Slice Architecture** where each feature is self-contained with its own:
- Command/Query (CQRS pattern with MediatR)
- Validator (FluentValidation)
- Handler
- Response DTOs

### Project Structure

```
src/Zento.Api/
├── Common/
│   ├── Behaviors/          # MediatR pipeline behaviors (validation)
│   └── Models/             # Shared models (Result, PagedResult)
├── Domain/
│   └── Entities/           # Domain entities (Product, Category, Supplier)
├── Features/
│   ├── Products/           # Product feature slices
│   ├── Categories/         # Category feature slices
│   └── Suppliers/          # Supplier feature slices
├── Endpoints/              # Minimal API endpoint mappings
├── Infrastructure/
│   └── Data/               # DbContext and data access
└── Program.cs              # Application entry point
```

## Tech Stack

- **.NET 8** - Minimal API
- **MediatR** - CQRS and request/response handling
- **FluentValidation** - Request validation
- **Entity Framework Core** - ORM
- **SQLite** - Database (for POC simplicity)
- **Swagger/OpenAPI** - API documentation

## Getting Started

### Prerequisites

- .NET 8 SDK or later

### Run the API

```bash
cd src/Zento.Api
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### Development

```bash
dotnet watch run
```

## API Endpoints

### Products
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products?page=1&pageSize=10&search=` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

### Categories
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories?page=1&pageSize=10&search=` | Get all categories |
| GET | `/api/categories/{id}` | Get category by ID |
| POST | `/api/categories` | Create a category |
| PUT | `/api/categories/{id}` | Update a category |
| DELETE | `/api/categories/{id}` | Delete a category |

### Suppliers
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/suppliers?page=1&pageSize=10&search=` | Get all suppliers |
| GET | `/api/suppliers/{id}` | Get supplier by ID |
| POST | `/api/suppliers` | Create a supplier |
| PUT | `/api/suppliers/{id}` | Update a supplier |
| DELETE | `/api/suppliers/{id}` | Delete a supplier |

### Health
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Health check |

## Sample Requests

### Create a Category
```bash
curl -X POST https://localhost:5001/api/categories \
  -H "Content-Type: application/json" \
  -d '{"name": "Electronics", "description": "Electronic devices and components"}'
```

### Create a Product
```bash
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "description": "High performance laptop",
    "sku": "LAP-001",
    "price": 999.99,
    "quantity": 50,
    "minimumStock": 10,
    "categoryId": "CATEGORY_ID_HERE"
  }'
```

## License

MIT
