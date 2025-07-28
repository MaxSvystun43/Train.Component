namespace Train.Component.Management.Database.Entities;

public class Item
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string UniqueNumber { get; set; }
    public required bool CanAssignQuantity { get; set; }
    
    public required ItemQuantity ItemQuantity { get; set; }
}