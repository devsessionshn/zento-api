using FluentValidation;
using MediatR;
using Zento.Api.Common.Models;

namespace Zento.Api.Features.Categories;

// Query
public record GetCategoriesQuery(int Page = 1, int PageSize = 10, string? Search = null) : IRequest<Result<PagedResult<CategoryDto>>>;

// Response
public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    int ProductCount,
    DateTime CreatedAt);

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
public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<PagedResult<CategoryDto>>>{
    private readonly ICategoryRepository _repository;

    public GetCategoriesHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var pagedCategories = await _repository.GetCategoriesAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken);

    // mapping Category to CategoryDto
        var items = pagedCategories.Data
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.IsActive,
                category.Products.Count,
                category.CreatedAt))
            .ToList();
    

        return Result<PagedResult<CategoryDto>>.Success(new PagedResult<CategoryDto>(
            items,
            pagedCategories.TotalCount,
            pagedCategories.Page,
            pagedCategories.PageSize));
    }
}
