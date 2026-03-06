using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Products;

// Query
public record GetProductsQuery(int Page = 1, int PageSize = 10, string? Search = null) : IRequest<GetProductsResponse>;

// Response
public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    int Quantity,
    int MinimumStock,
    Guid CategoryId,
    string? CategoryName,
    bool IsActive,
    DateTime CreatedAt);

public record GetProductsResponse(
    IReadOnlyList<ProductDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

// Validator
public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

// Handler
public class GetProductsHandler : IRequestHandler<GetProductsQuery, GetProductsResponse>
{
    private readonly ZentoDbContext _context;

    public GetProductsHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<GetProductsResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p => 
                p.Name.Contains(request.Search) || 
                p.Sku.Contains(request.Search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new GetProductsResponse(items, totalCount, request.Page, request.PageSize, totalPages);
    }
}
