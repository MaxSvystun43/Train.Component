namespace Train.Component.Management.Service.Models;

public class ItemWithQuantity
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string UniqueNumber { get; set; }
    public required bool CanAssignQuantity { get; set; }
    public required int? CurrentQuantity { get; set; }
}