using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Domain.Entities;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Categories;

// Command
public record CreateCategoryCommand(
    string Name,
    string? Description) : IRequest<CreateCategoryResponse>;

// Response
public record CreateCategoryResponse(Guid Id, string Name);

// Validator
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly ZentoDbContext _context;

    public CreateCategoryCommandValidator(ZentoDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters")
            .MustAsync(BeUniqueName).WithMessage("Category name already exists");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Categories.AnyAsync(c => c.Name == name, cancellationToken);
    }
}

// Handler
public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    private readonly ZentoDbContext _context;

    public CreateCategoryHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateCategoryResponse(category.Id, category.Name);
    }
}
