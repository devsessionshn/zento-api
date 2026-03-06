using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Products;

// Command
public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    int Quantity,
    int MinimumStock,
    Guid CategoryId,
    bool IsActive) : IRequest<UpdateProductResponse?>;

// Response
public record UpdateProductResponse(Guid Id, string Name, string Sku, DateTime UpdatedAt);

// Validator
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly ZentoDbContext _context;

    public UpdateProductCommandValidator(ZentoDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters")
            .MustAsync(BeUniqueSkuForUpdate).WithMessage("SKU already exists");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock must be non-negative");

        RuleFor(x => x.CategoryId)
            .MustAsync(CategoryExists).WithMessage("Category does not exist");
    }

    private async Task<bool> BeUniqueSkuForUpdate(UpdateProductCommand command, string sku, CancellationToken cancellationToken)
    {
        return !await _context.Products.AnyAsync(p => p.Sku == sku && p.Id != command.Id, cancellationToken);
    }

    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }
}

// Handler
public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse?>
{
    private readonly ZentoDbContext _context;

    public UpdateProductHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateProductResponse?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync([request.Id], cancellationToken);

        if (product == null)
            return null;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Sku = request.Sku;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.MinimumStock = request.MinimumStock;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateProductResponse(product.Id, product.Name, product.Sku, product.UpdatedAt.Value);
    }
}
