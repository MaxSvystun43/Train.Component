namespace Train.Component.Management.Database.Entities;

public class ItemQuantity
{
    public long Id { get; set; }
    public required Item Item { get; set; }
    public required long ItemId { get; set; }
    public required int Quantity { get; set; }
}