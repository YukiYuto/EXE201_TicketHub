using System.ComponentModel.DataAnnotations;
using TicketHub.Utility.ValidationAttribute;

namespace TicketHub.Models.DTO.Authentication;

public class ChangePasswordDto
{
    [Required]
    [DataType(DataType.Password)]
    [Password]
    public string OldPassword { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Password]
    public string NewPassword { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [ConfirmPassword("NewPassword")]
    public string ConfirmNewPassword { get; set; } = null!;
}