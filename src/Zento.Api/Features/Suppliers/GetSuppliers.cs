using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Suppliers;

// Query
public record GetSuppliersQuery(int Page = 1, int PageSize = 10, string? Search = null) : IRequest<GetSuppliersResponse>;

// Response
public record SupplierDto(
    Guid Id,
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime CreatedAt);

public record GetSuppliersResponse(
    IReadOnlyList<SupplierDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

// Validator
public class GetSuppliersQueryValidator : AbstractValidator<GetSuppliersQuery>
{
    public GetSuppliersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

// Handler
public class GetSuppliersHandler : IRequestHandler<GetSuppliersQuery, GetSuppliersResponse>
{
    private readonly ZentoDbContext _context;

    public GetSuppliersHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<GetSuppliersResponse> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(s => 
                s.Name.Contains(request.Search) || 
                (s.ContactName != null && s.ContactName.Contains(request.Search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SupplierDto(
                s.Id,
                s.Name,
                s.ContactName,
                s.Email,
                s.Phone,
                s.Address,
                s.IsActive,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new GetSuppliersResponse(items, totalCount, request.Page, request.PageSize, totalPages);
    }
}
