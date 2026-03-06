using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Categories;

// Command
public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive) : IRequest<UpdateCategoryResponse?>;

// Response
public record UpdateCategoryResponse(Guid Id, string Name, DateTime UpdatedAt);

// Validator
public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly ZentoDbContext _context;

    public UpdateCategoryCommandValidator(ZentoDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters")
            .MustAsync(BeUniqueNameForUpdate).WithMessage("Category name already exists");
    }

    private async Task<bool> BeUniqueNameForUpdate(UpdateCategoryCommand command, string name, CancellationToken cancellationToken)
    {
        return !await _context.Categories.AnyAsync(c => c.Name == name && c.Id != command.Id, cancellationToken);
    }
}

// Handler
public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse?>
{
    private readonly ZentoDbContext _context;

    public UpdateCategoryHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateCategoryResponse?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync([request.Id], cancellationToken);

        if (category == null)
            return null;

        category.Name = request.Name;
        category.Description = request.Description;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateCategoryResponse(category.Id, category.Name, category.UpdatedAt.Value);
    }
}
