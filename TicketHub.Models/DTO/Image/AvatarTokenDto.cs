namespace TicketHub.Models.DTO.Image;

public class AvatarTokenDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
}