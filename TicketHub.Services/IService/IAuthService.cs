using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Authentication;
using TicketHub.Models.DTO.Email;

namespace TicketHub.Services.IService;

public interface IAuthService
{
    Task<ResponseDto> SignUpCustomer(SignUpCustomerDto signUpCustomerDto);
    Task<ResponseDto> SignUpOrganization(SignUpOrganizationDto signUpOrganizationDto);
    Task<ResponseDto> SignIn(SignInDto signDto);
    /*Task<ResponseDto> SignInByGoogle(SignInByGoogleDto signInByGoogleDto);*/
    Task<ResponseDto> UpdateUserProfile(ClaimsPrincipal userPrincipal,UpdateUserProfileDto updateUserProfileDto);
    Task<ResponseDto> GetUserById(Guid userId);
    Task<ResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto);
    Task<ResponseDto> FetchUserByToken(ClaimsPrincipal user);
    Task<ResponseDto> SendVerifyEmail(EmailDto emailDto);
    Task<ResponseDto> VerifyEmail(VerifyEmailDto verifyEmailDto);
    Task<ResponseDto> ChangePassword(ClaimsPrincipal userPrincipal, ChangePasswordDto changePasswordDto);
    Task<ResponseDto> ForgotPassword(EmailDto forgotPasswordDto);
    Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto);
    /*Task<ResponseDto> UploadUserAvatar(IFormFile file, ClaimsPrincipal user);
    Task<MemoryStream> GetUserAvatar(ClaimsPrincipal user);
    Task<ResponseDto> LockUser(string id);
    Task<ResponseDto> UnlockUser(string id);*/
}