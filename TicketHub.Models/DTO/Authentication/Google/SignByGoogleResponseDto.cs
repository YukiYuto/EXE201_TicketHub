namespace TicketHub.Models.DTO.Authentication.Google;

public class SignByGoogleResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public bool IsProfileComplete { get; set; }
}