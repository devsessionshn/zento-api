using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Products;

// Query
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

// Handler
public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly ZentoDbContext _context;

    public GetProductByIdHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Sku,
                p.Price,
                p.Quantity,
                p.MinimumStock,
                p.CategoryId,
                p.Category != null ? p.Category.Name : null,
                p.IsActive,
                p.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
