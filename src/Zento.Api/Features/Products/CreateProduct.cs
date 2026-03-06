using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Domain.Entities;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Products;

// Command
public record CreateProductCommand(
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    int Quantity,
    int MinimumStock,
    Guid CategoryId) : IRequest<CreateProductResponse>;

// Response
public record CreateProductResponse(Guid Id, string Name, string Sku);

// Validator
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly ZentoDbContext _context;

    public CreateProductCommandValidator(ZentoDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters")
            .MustAsync(BeUniqueSku).WithMessage("SKU already exists");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock must be non-negative");

        RuleFor(x => x.CategoryId)
            .MustAsync(CategoryExists).WithMessage("Category does not exist");
    }

    private async Task<bool> BeUniqueSku(string sku, CancellationToken cancellationToken)
    {
        return !await _context.Products.AnyAsync(p => p.Sku == sku, cancellationToken);
    }

    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }
}

// Handler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly ZentoDbContext _context;

    public CreateProductHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Sku = request.Sku,
            Price = request.Price,
            Quantity = request.Quantity,
            MinimumStock = request.MinimumStock,
            CategoryId = request.CategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id, product.Name, product.Sku);
    }
}
