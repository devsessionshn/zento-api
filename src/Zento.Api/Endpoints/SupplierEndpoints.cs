using MediatR;
using Zento.Api.Features.Suppliers;

namespace Zento.Api.Endpoints;

public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers")
            .WithTags("Suppliers");

        group.MapGet("/", async (
            int page,
            int pageSize,
            string? search,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetSuppliersQuery(page, pageSize, search), ct);
            return Results.Ok(result);
        })
        .WithName("GetSuppliers")
        .WithDescription("Get all suppliers with pagination");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetSupplierByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetSupplierById")
        .WithDescription("Get a supplier by ID");

        group.MapPost("/", async (
            CreateSupplierCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/suppliers/{result.Id}", result);
        })
        .WithName("CreateSupplier")
        .WithDescription("Create a new supplier");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateSupplierRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new UpdateSupplierCommand(
                id,
                request.Name,
                request.ContactName,
                request.Email,
                request.Phone,
                request.Address,
                request.IsActive);

            var result = await mediator.Send(command, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("UpdateSupplier")
        .WithDescription("Update an existing supplier");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteSupplierCommand(id), ct);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteSupplier")
        .WithDescription("Delete a supplier");
    }
}

public record UpdateSupplierRequest(
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive);
