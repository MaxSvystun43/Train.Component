using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Train.Component.Management.Database;
using Train.Component.Management.Database.Entities;
using Train.Component.Management.Service.Models;

namespace Train.Component.Management.Service;

public class ItemService(ManagementDbContext context, ILogger<ItemService> logger) : IItemService
{
    public async Task<IEnumerable<ItemWithQuantity>> GetAllItemsAsync()
    {
        logger.LogInformation("Getting all items");
        
        return await context.Items
            .Include(i => i.ItemQuantity)
            .Select(i => new ItemWithQuantity
            {
                Id = i.Id,
                Name = i.Name,
                UniqueNumber = i.UniqueNumber,
                CanAssignQuantity = i.CanAssignQuantity,
                CurrentQuantity = i.CanAssignQuantity ? i.ItemQuantity.Quantity : null
            })
            .ToListAsync();
    }

    public async Task<ItemWithQuantity?> GetItemByIdAsync(long id)
    {
        logger.LogInformation("Getting item by id: {ItemId}", id);
        
        var item = await context.Items
            .Include(i => i.ItemQuantity)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            logger.LogInformation("Item with id: {ItemId} not found", id);
            return null;
        }

        return new ItemWithQuantity
        {
            Id = item.Id,
            Name = item.Name,
            UniqueNumber = item.UniqueNumber,
            CanAssignQuantity = item.CanAssignQuantity,
            CurrentQuantity = item.CanAssignQuantity ? item.ItemQuantity.Quantity : null
        };
    }

    public async Task<IEnumerable<ItemWithQuantity>> SearchItemsAsync(string searchTerm)
    {
        logger.LogInformation("Searching items for search term: {SearchTerm}", searchTerm);
        
        var query = context.Items
            .Include(i => i.ItemQuantity)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(i => 
                i.Name.ToLower().Contains(searchTerm) || 
                i.UniqueNumber.ToLower().Contains(searchTerm));
        }

        return await query
            .Select(i => new ItemWithQuantity
            {
                Id = i.Id,
                Name = i.Name,
                UniqueNumber = i.UniqueNumber,
                CanAssignQuantity = i.CanAssignQuantity,
                CurrentQuantity = i.CanAssignQuantity ? i.ItemQuantity.Quantity : null
            })
            .ToListAsync();
    }

    public async Task<ItemWithQuantity> CreateItemAsync(CreateItemRequest request)
    {
        logger.LogInformation("Creating item");
        logger.LogDebug("Create request {ItemRequest}", request);
        
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var item = new Item
            {
                Name = request.Name,
                UniqueNumber = request.UniqueNumber,
                CanAssignQuantity = request.CanAssignQuantity,
                ItemQuantity = null!
            };

            context.Items.Add(item);
            await context.SaveChangesAsync();

            var itemQuantity = new ItemQuantity
            {
                ItemId = item.Id,
                Item = item,
                Quantity = request.CanAssignQuantity ? (request.InitialQuantity ?? 0) : 0
            };

            context.ItemQuantities.Add(itemQuantity);
            item.ItemQuantity = itemQuantity;
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            logger.LogInformation("Created item {ItemId}", item.Id);

            return new ItemWithQuantity
            {
                Id = item.Id,
                Name = item.Name,
                UniqueNumber = item.UniqueNumber,
                CanAssignQuantity = item.CanAssignQuantity,
                CurrentQuantity = item.CanAssignQuantity ? itemQuantity.Quantity : null
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ItemWithQuantity> UpdateItemAsync(long id, UpdateItemRequest request)
    {
        logger.LogInformation("Updating item with id: {ItemId}", id);
        var item = await context.Items
            .Include(i => i.ItemQuantity)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            logger.LogInformation("Item with id: {ItemId} not found", id);
            throw new ArgumentException($"Item with ID {id} not found");
        }

        item.Name = request.Name;
        item.UniqueNumber = request.UniqueNumber;
        item.CanAssignQuantity = request.CanAssignQuantity;

        if (!request.CanAssignQuantity)
        {
            item.ItemQuantity.Quantity = 0;
        }

        await context.SaveChangesAsync();
        
        logger.LogInformation("Updated item with id: {ItemId}", id);
        return new ItemWithQuantity
        {
            Id = item.Id,
            Name = item.Name,
            UniqueNumber = item.UniqueNumber,
            CanAssignQuantity = item.CanAssignQuantity,
            CurrentQuantity = item.CanAssignQuantity ? item.ItemQuantity.Quantity : null
        };
    }

    public async Task<bool> UpdateItemQuantityAsync(long itemId, int quantity)
    {
        logger.LogInformation("Updating item with id: {ItemId} quantity: {Quantity}", itemId, quantity);
        if (quantity < 0)
        {
            logger.LogInformation("Item with id: {ItemId} quantity cannot be negative", itemId);
            throw new ArgumentException("Quantity must be a positive integer");
        }

        var item = await context.Items
            .Include(i => i.ItemQuantity)
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item is not { CanAssignQuantity: true }) 
            return false;

        item.ItemQuantity.Quantity = quantity;
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteItemAsync(long id)
    {
        logger.LogInformation("Deleting item with id: {ItemId}", id);
        
        var item = await context.Items
            .Include(i => i.ItemQuantity)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            logger.LogInformation("Item with id: {ItemId} not found", id);
            return false;
        }

        context.Items.Remove(item);
        await context.SaveChangesAsync();
        logger.LogInformation("Deleted item with id: {ItemId}", id);

        return true;
    }
}