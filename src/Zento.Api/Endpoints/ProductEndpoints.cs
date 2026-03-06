using MediatR;
using Zento.Api.Features.Products;

namespace Zento.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", async (
            int page,
            int pageSize,
            string? search,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetProductsQuery(page, pageSize, search), ct);
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithDescription("Get all products with pagination");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetProductByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetProductById")
        .WithDescription("Get a product by ID");

        group.MapPost("/", async (
            CreateProductCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/products/{result.Id}", result);
        })
        .WithName("CreateProduct")
        .WithDescription("Create a new product");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateProductRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new UpdateProductCommand(
                id,
                request.Name,
                request.Description,
                request.Sku,
                request.Price,
                request.Quantity,
                request.MinimumStock,
                request.CategoryId,
                request.IsActive);

            var result = await mediator.Send(command, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("UpdateProduct")
        .WithDescription("Update an existing product");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteProductCommand(id), ct);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithDescription("Delete a product");
    }
}

public record UpdateProductRequest(
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    int Quantity,
    int MinimumStock,
    Guid CategoryId,
    bool IsActive);
