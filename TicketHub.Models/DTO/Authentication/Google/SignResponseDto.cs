namespace TicketHub.Models.DTO.Authentication.Google;

public class SignResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}