using System.ComponentModel.DataAnnotations;

namespace Train.Component.Management.Service.Models;

public class CreateItemRequest
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    [StringLength(100)]
    public required string UniqueNumber { get; set; }

    public required bool CanAssignQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Initial quantity must be a positive integer")]
    public int? InitialQuantity { get; set; }
}