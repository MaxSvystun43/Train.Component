using Train.Component.Management.Service.Models;

namespace Train.Component.Management.Service;

public interface IItemService
{
    Task<IEnumerable<ItemWithQuantity>> GetAllItemsAsync();
    Task<ItemWithQuantity?> GetItemByIdAsync(long id);
    Task<IEnumerable<ItemWithQuantity>> SearchItemsAsync(string searchTerm);
    Task<ItemWithQuantity> CreateItemAsync(CreateItemRequest request);
    Task<ItemWithQuantity> UpdateItemAsync(long id, UpdateItemRequest request);
    Task<bool> UpdateItemQuantityAsync(long itemId, int quantity);
    Task<bool> DeleteItemAsync(long id);
}