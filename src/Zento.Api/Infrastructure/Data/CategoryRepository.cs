using Microsoft.EntityFrameworkCore;
using Zento.Api.Common.Extensions;
using Zento.Api.Common.Models;
using Zento.Api.Domain.Entities;
using Zento.Api.Features.Categories;

namespace Zento.Api.Infrastructure.Data;

public class CategoryRepository : ICategoryRepository
{
    private readonly ZentoDbContext _context;

    public CategoryRepository(ZentoDbContext context)
    {
        _context = context;
    }

    public Task<PagedResult<Category>> GetCategoriesAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Include(category => category.Products)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(category => category.Name.Contains(search));
        }

        return query
            .OrderBy(category => category.Name)
            .ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}