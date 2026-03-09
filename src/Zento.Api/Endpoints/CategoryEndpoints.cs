using MediatR;
using Zento.Api.Features.Categories;

namespace Zento.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories");

        group.MapGet("/", async (
            int page,
            int pageSize,
            string? search,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCategoriesQuery(page, pageSize, search), ct);
            return result.IsSuccess && result.Value is not null
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { result.Error });
        })
        .WithName("GetCategories")
        .WithDescription("Get all categories with pagination");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCategoryByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetCategoryById")
        .WithDescription("Get a category by ID");

        group.MapPost("/", async (
            CreateCategoryCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/categories/{result.Id}", result);
        })
        .WithName("CreateCategory")
        .WithDescription("Create a new category");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCategoryRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new UpdateCategoryCommand(id, request.Name, request.Description, request.IsActive);
            var result = await mediator.Send(command, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("UpdateCategory")
        .WithDescription("Update an existing category");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteCategoryCommand(id), ct);
            return result.Success ? Results.NoContent() : Results.BadRequest(new { result.Error });
        })
        .WithName("DeleteCategory")
        .WithDescription("Delete a category");
    }
}

public record UpdateCategoryRequest(string Name, string? Description, bool IsActive);
