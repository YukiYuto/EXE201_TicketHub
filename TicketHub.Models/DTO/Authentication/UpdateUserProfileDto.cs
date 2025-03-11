namespace TicketHub.Models.DTO.Authentication;

public class UpdateUserProfileDto
{
    public DateTime? BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }

    // Các trường dành cho người dùng
    public string? FullName { get; set; }
    public string? CCCD { get; set; }
    public string? Gender { get; set; }

    // Các trường dành cho nhà tổ chức sự kiện
    public string? OrganizationName { get; set; }
    public string? TaxId { get; set; }
}