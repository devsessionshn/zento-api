using MediatR;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Suppliers;

// Command
public record DeleteSupplierCommand(Guid Id) : IRequest<bool>;

// Handler
public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand, bool>
{
    private readonly ZentoDbContext _context;

    public DeleteSupplierHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers.FindAsync([request.Id], cancellationToken);

        if (supplier == null)
            return false;

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
