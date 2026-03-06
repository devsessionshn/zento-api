using MediatR;
using Zento.Api.Infrastructure.Data;

namespace Zento.Api.Features.Products;

// Command
public record DeleteProductCommand(Guid Id) : IRequest<bool>;

// Handler
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly ZentoDbContext _context;

    public DeleteProductHandler(ZentoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync([request.Id], cancellationToken);

        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
