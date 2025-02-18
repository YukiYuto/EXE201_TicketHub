namespace TicketHub.Models.DTO.Authentication;

public class GetUserDto
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Address { get; set; } = null!;
    public string? ImageUrl { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string CCCD { get; set; } = null!;
    public string OrganizationName { get; set; } = null!;
    public string TaxId { get; set; } = null!;
    public List<string> Roles { get; set; } = new List<string>();
}