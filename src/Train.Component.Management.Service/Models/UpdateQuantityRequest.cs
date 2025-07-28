using System.ComponentModel.DataAnnotations;

namespace Train.Component.Management.Service.Models;

public class UpdateQuantityRequest
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive integer")]
    public required int Quantity { get; set; }
}