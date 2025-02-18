using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketHub.Utility.ValidationAttribute;

namespace TicketHub.Models.DTO.Authentication;

public class SignUpCustomerDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Password]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [ConfirmPassword("Password")]
    [NotMapped]
    public string ConfirmPassword { get; set; } = null!;
 
    [Required]
    [Cccd]
    public string CCCD { get; set; } = null!;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime BirthDate { get; set; }

    [Required]
    [DataType(DataType.PhoneNumber)]
    [Phone]
    public string PhoneNumber { get; set; } = null!;
 
    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public string Country { get; set; } = null!;
 
    [Required]
    public string Address { get; set; } = null!;
}