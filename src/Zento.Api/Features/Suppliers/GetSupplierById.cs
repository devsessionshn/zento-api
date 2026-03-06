using MediatR;
using Microsoft.EntityFrameworkCore;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Suppliers;

// Query
public record GetSupplierByIdQuery(Guid Id) : IRequest<SupplierDto?>;

// Handler
public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly ZentoDbContext _context;

    public GetSupplierByIdHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .Where(s => s.Id == request.Id)
            .Select(s => new SupplierDto(
                s.Id,
                s.Name,
                s.ContactName,
                s.Email,
                s.Phone,
                s.Address,
                s.IsActive,
                s.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
