namespace TicketHub.Models.DTO.Customer;

public class CustomerGetAllDto
{
    public Guid CustomerId { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? CCCD { get; set; }
    public string? Gender { get; set; }
}