using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Categories;

// Query
public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;

// Handler
public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ZentoDbContext _context;

    public GetCategoryByIdHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.IsActive,
                c.Products.Count,
                c.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
