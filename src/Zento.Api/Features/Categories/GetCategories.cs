using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Categories;

// Query
public record GetCategoriesQuery(int Page = 1, int PageSize = 10, string? Search = null) : IRequest<GetCategoriesResponse>;

// Response
public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    int ProductCount,
    DateTime CreatedAt);

public record GetCategoriesResponse(
    IReadOnlyList<CategoryDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

// Validator
public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

// Handler
public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, GetCategoriesResponse>
{
    private readonly ZentoDbContext _context;

    public GetCategoriesHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<GetCategoriesResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c => c.Name.Contains(request.Search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.IsActive,
                c.Products.Count,
                c.CreatedAt))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new GetCategoriesResponse(items, totalCount, request.Page, request.PageSize, totalPages);
    }
}
