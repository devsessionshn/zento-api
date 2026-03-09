using Microsoft.EntityFrameworkCore;
using Zento.Api.Domain.Entities;

namespace Zento.Api.Infrastructure.Data;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(ZentoDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Categories.AnyAsync(cancellationToken) ||
            await dbContext.Products.AnyAsync(cancellationToken) ||
            await dbContext.Suppliers.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var electronicsId = Guid.Parse("2f968f64-5bf8-4284-bf83-8f41f830211f");
        var officeId = Guid.Parse("8af30d8a-6d47-4e86-a8e0-a4d4f2825fa8");
        var groceryId = Guid.Parse("af19582a-3d22-45bf-bf7c-4daa603f258f");

        var categories = new List<Category>
        {
            new()
            {
                Id = electronicsId,
                Name = "Electronics",
                Description = "Devices and accessories",
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = officeId,
                Name = "Office",
                Description = "Office supplies and equipment",
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = groceryId,
                Name = "Grocery",
                Description = "Food and beverage products",
                IsActive = true,
                CreatedAt = now
            }
        };

        var suppliers = new List<Supplier>
        {
            new()
            {
                Id = Guid.Parse("28bceec7-ec30-4154-aa90-103f023dbeb9"),
                Name = "Northwind Traders",
                ContactName = "Alicia Romero",
                Email = "sales@northwindtraders.test",
                Phone = "+1-555-1000",
                Address = "120 Main St, Redmond",
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.Parse("75e2ca92-fb39-430f-8aee-44c0ad72df9f"),
                Name = "Contoso Supplies",
                ContactName = "David Herrera",
                Email = "contact@contososupplies.test",
                Phone = "+1-555-2000",
                Address = "85 Pine Ave, Seattle",
                IsActive = true,
                CreatedAt = now
            }
        };

        var products = new List<Product>
        {
            new()
            {
                Id = Guid.Parse("de7ed952-f5a4-4f8a-b5ad-b0b3bd0f0e3d"),
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse",
                Sku = "ELEC-MSE-001",
                Price = 29.99m,
                Quantity = 45,
                MinimumStock = 10,
                CategoryId = electronicsId,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.Parse("ec802504-03cb-48c1-82bc-3eb6435c7c89"),
                Name = "Mechanical Keyboard",
                Description = "Backlit mechanical keyboard",
                Sku = "ELEC-KBD-001",
                Price = 89.00m,
                Quantity = 18,
                MinimumStock = 5,
                CategoryId = electronicsId,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.Parse("7f4eb752-3cb8-4f53-aef2-dd8b4ad56d2b"),
                Name = "A4 Copy Paper",
                Description = "500-sheet ream",
                Sku = "OFF-PPR-001",
                Price = 7.50m,
                Quantity = 120,
                MinimumStock = 25,
                CategoryId = officeId,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.Parse("55073864-b35f-4862-9cea-cee53580f26f"),
                Name = "Sparkling Water",
                Description = "12-pack cans",
                Sku = "GRC-WTR-001",
                Price = 6.40m,
                Quantity = 70,
                MinimumStock = 15,
                CategoryId = groceryId,
                IsActive = true,
                CreatedAt = now
            }
        };

        await dbContext.Categories.AddRangeAsync(categories, cancellationToken);
        await dbContext.Suppliers.AddRangeAsync(suppliers, cancellationToken);
        await dbContext.Products.AddRangeAsync(products, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
