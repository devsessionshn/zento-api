using FluentValidation;
using MediatR;
using Zento.Api.Domain.Entities;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Suppliers;

// Command
public record CreateSupplierCommand(
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address) : IRequest<CreateSupplierResponse>;

// Response
public record CreateSupplierResponse(Guid Id, string Name);

// Validator
public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Email)
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters")
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format");
    }
}

// Handler
public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResponse>
{
    private readonly ZentoDbContext _context;

    public CreateSupplierHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<CreateSupplierResponse> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateSupplierResponse(supplier.Id, supplier.Name);
    }
}
