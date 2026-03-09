using Zento.Api.Domain.Entities;
using Zento.Api.Common.Models;

namespace Zento.Api.Features.Categories;

public interface ICategoryRepository
{
    Task<PagedResult<Category>> GetCategoriesAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);
}