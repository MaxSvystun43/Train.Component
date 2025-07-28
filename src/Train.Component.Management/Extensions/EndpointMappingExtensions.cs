using Train.Component.Management.Service;
using Train.Component.Management.Service.Models;

namespace Train.Component.Management.Extensions;

internal static class EndpointMappingExtensions
{
    public static void AddEndpoints(this WebApplication app)
    {
        app.MapGet("/api/items", async (IItemService itemService) =>
            Results.Ok(await itemService.GetAllItemsAsync()));

        app.MapGet("/api/items/{id:long}", async (long id, IItemService itemService) =>
            await itemService.GetItemByIdAsync(id) is { }
                item ? Results.Ok(item) : Results.NotFound());

        app.MapGet("/api/items/search", async (string? term, IItemService itemService) =>
            Results.Ok(await itemService.SearchItemsAsync(term ?? string.Empty)));

        app.MapPost("/api/items", async (CreateItemRequest request, IItemService itemService) =>
        {
            try
            {
                var item = await itemService.CreateItemAsync(request);
                return Results.Ok(item);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        app.MapPut("/api/items/{id:long}", async (long id, UpdateItemRequest request, IItemService itemService) =>
        {
            try
            {
                var item = await itemService.UpdateItemAsync(id, request);
                return Results.Ok(item);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        app.MapPatch("/api/items/{id:long}/quantity",
            async (long id, UpdateQuantityRequest request, IItemService itemService) =>
            {
                try
                {
                    var success = await itemService.UpdateItemQuantityAsync(id, request.Quantity);
                    return success ? Results.NoContent() : Results.NotFound();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

        app.MapDelete("/api/items/{id:long}", async (long id, IItemService itemService) =>
            await itemService.DeleteItemAsync(id) ? Results.NoContent() : Results.NotFound());
    }
}