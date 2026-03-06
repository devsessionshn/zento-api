using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Categories;

// Command
public record DeleteCategoryCommand(Guid Id) : IRequest<DeleteCategoryResult>;

// Response
public record DeleteCategoryResult(bool Success, string? Error = null);

// Handler
public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResult>
{
    private readonly ZentoDbContext _context;

    public DeleteCategoryHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteCategoryResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            return new DeleteCategoryResult(false, "Category not found");

        if (category.Products.Any())
            return new DeleteCategoryResult(false, "Cannot delete category with associated products");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteCategoryResult(true);
    }
}
