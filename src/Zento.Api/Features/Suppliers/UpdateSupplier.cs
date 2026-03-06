using FluentValidation;
using MediatR;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Suppliers;

// Command
public record UpdateSupplierCommand(
    Guid Id,
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive) : IRequest<UpdateSupplierResponse?>;

// Response
public record UpdateSupplierResponse(Guid Id, string Name, DateTime UpdatedAt);

// Validator
public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
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
public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand, UpdateSupplierResponse?>
{
    private readonly ZentoDbContext _context;

    public UpdateSupplierHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateSupplierResponse?> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers.FindAsync([request.Id], cancellationToken);

        if (supplier == null)
            return null;

        supplier.Name = request.Name;
        supplier.ContactName = request.ContactName;
        supplier.Email = request.Email;
        supplier.Phone = request.Phone;
        supplier.Address = request.Address;
        supplier.IsActive = request.IsActive;
        supplier.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateSupplierResponse(supplier.Id, supplier.Name, supplier.UpdatedAt.Value);
    }
}
