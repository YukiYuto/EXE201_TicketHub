using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Authentication;
using TicketHub.Models.DTO.Email;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.Services.Service;

public class AuthService : IAuthService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IEmailService _emailService;

    public AuthService
    (
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IEmailService emailService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _tokenHandler = new JwtSecurityTokenHandler();
        _unitOfWork = unitOfWork;
        /*_mapperService = mapperService;*/
        _emailService = emailService;
    }

    public async Task<ResponseDto> SignUpCustomer(SignUpCustomerDto signUpCustomerDto)
    {
        // Kiểm tra email đã tồn tại
        var isEmailExit = await _userManager.FindByEmailAsync(signUpCustomerDto.Email);
        if (isEmailExit is not null)
        {
            return new ResponseDto()
            {
                Message = "Email is being used by another user",
                Result = signUpCustomerDto,
                IsSuccess = false,
                StatusCode = 400
            };
        }

        // Kiểm tra số điện thoại đã tồn tại
        var isPhoneNumberExit = await _userManager.Users
            .AnyAsync(u => u.PhoneNumber == signUpCustomerDto.PhoneNumber);
        if (isPhoneNumberExit)
        {
            return new ResponseDto()
            {
                Message = "Phone number is being used by another user",
                Result = signUpCustomerDto,
                IsSuccess = false,
                StatusCode = 400
            };
        }

        // Tạo đối tượng ApplicationUser mới
        ApplicationUser newUser = new ApplicationUser()
        {
            Email = signUpCustomerDto.Email,
            UserName = signUpCustomerDto.Email,
            FullName = signUpCustomerDto.FullName,
            Address = signUpCustomerDto.Address,
            Country = signUpCustomerDto.Country,
            BirthDate = signUpCustomerDto.BirthDate,
            PhoneNumber = signUpCustomerDto.PhoneNumber,
            CCCD = signUpCustomerDto.CCCD,
            AvatarUrl = "",
            LockoutEnabled = false
        };

        // Thêm người dùng mới vào database
        var createUserResult = await _userManager.CreateAsync(newUser, signUpCustomerDto.Password);

        // Kiểm tra lỗi khi tạo
        if (!createUserResult.Succeeded)
        {
            return new ResponseDto()
            {
                Message = "Create user failed",
                IsSuccess = false,
                StatusCode = 400,
                Result = signUpCustomerDto
            };
        }

        var user = newUser;
        var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.Member);

        if (!isRoleExist)
        {
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Member));
        }


        // Thêm role "Customer" cho người dùng
        var isRoleAdded = await _userManager.AddToRoleAsync(newUser, StaticUserRoles.Member);

        if (!isRoleAdded.Succeeded)
        {
            return new ResponseDto()
            {
                Message = "Error adding role",
                IsSuccess = false,
                StatusCode = 500,
                Result = signUpCustomerDto
            };
        }

        await _unitOfWork.SaveAsync();

        return new ResponseDto()
        {
            Message = "User created successfully",
            IsSuccess = true,
            StatusCode = 201,
            Result = signUpCustomerDto
        };
    }


    public async Task<ResponseDto> SignUpOrganization(SignUpOrganizationDto signUpOrganizationDto)
    {
        // Kiểm tra email đã tồn tại
        var isEmailExit = await _userManager.FindByEmailAsync(signUpOrganizationDto.Email);
        if (isEmailExit is not null)
        {
            return new ResponseDto()
            {
                Message = "Email is being used by another user",
                Result = signUpOrganizationDto,
                IsSuccess = false,
                StatusCode = 400
            };
        }

        // Kiểm tra số điện thoại đã tồn tại
        var isPhoneNumberExit = await _userManager.Users
            .AnyAsync(u => u.PhoneNumber == signUpOrganizationDto.PhoneNumber);
        if (isPhoneNumberExit)
        {
            return new ResponseDto()
            {
                Message = "Phone number is being used by another user",
                Result = signUpOrganizationDto,
                IsSuccess = false,
                StatusCode = 400
            };
        }

        // Tạo đối tượng ApplicationUser mới
        ApplicationUser newUser = new ApplicationUser()
        {
            Email = signUpOrganizationDto.Email,
            UserName = signUpOrganizationDto.Email,
            OrganizationName = signUpOrganizationDto.OrganizationName,
            Address = signUpOrganizationDto.Address,
            Country = signUpOrganizationDto.Country,
            TaxId = signUpOrganizationDto.TaxId,
            PhoneNumber = signUpOrganizationDto.PhoneNumber,
            AvatarUrl = "",
            LockoutEnabled = false
        };

        // Thêm người dùng mới vào database
        var createUserResult = await _userManager.CreateAsync(newUser, signUpOrganizationDto.Password);

        // Kiểm tra lỗi khi tạo
        if (!createUserResult.Succeeded)
        {
            return new ResponseDto()
            {
                Message = "Create user failed",
                IsSuccess = false,
                StatusCode = 400,
                Result = signUpOrganizationDto
            };
        }

        var user = newUser;
        var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.Organization);

        if (!isRoleExist)
        {
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Organization));
        }


        // Thêm role "Customer" cho người dùng
        var isRoleAdded = await _userManager.AddToRoleAsync(newUser, StaticUserRoles.Organization);

        if (!isRoleAdded.Succeeded)
        {
            return new ResponseDto()
            {
                Message = "Error adding role",
                IsSuccess = false,
                StatusCode = 500,
                Result = signUpOrganizationDto
            };
        }

        await _unitOfWork.SaveAsync();

        return new ResponseDto()
        {
            Message = "User created successfully",
            IsSuccess = true,
            StatusCode = 201,
            Result = signUpOrganizationDto
        };
    }


    public async Task<ResponseDto> SignIn(SignInDto signDto)
    {
        var user = await _userManager.FindByEmailAsync(signDto.Email);
        if (user == null)
        {
            return new ResponseDto()
            {
                Message = "User does not exist!",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, signDto.Password);

        if (!isPasswordCorrect)
        {
            return new ResponseDto()
            {
                Message = "Incorrect email or password",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };
        }

        var role = await _userManager.GetRolesAsync(user);

        if (!user.EmailConfirmed)
        {
            return new ResponseDto()
            {
                Message = "You need to confirm email!",
                Result = null,
                IsSuccess = false,
                StatusCode = 401
            };
        }

        if (user.LockoutEnd is not null)
        {
            return new ResponseDto()
            {
                Message = "User has been locked",
                IsSuccess = false,
                StatusCode = 403,
                Result = null
            };
        }

        string accessToken;
        if (role.Contains(StaticUserRoles.Organization))
        {
            accessToken = await _tokenService.GenerateJwtAccessTokenOrganizationAsync(user);
        }
        else
        {
            accessToken = await _tokenService.GenerateJwtAccessTokenCustomerAsync(user);
        }

        var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
        await _tokenService.StoreRefreshToken(user.Id, refreshToken);

        return new ResponseDto()
        {
            Result = new SignInResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            },
            Message = "Sign in successfully",
            IsSuccess = true,
            StatusCode = 200
        };
    }


    public async Task<ResponseDto> UpdateUserProfile(ClaimsPrincipal userPrincipal,
        UpdateUserProfileDto updateUserProfileDto)
    {
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return new ResponseDto()
            {
                Message = "Unauthorized",
                StatusCode = 401,
                IsSuccess = false,
                Result = null
            };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ResponseDto
            {
                Message = "User not found",
                IsSuccess = false,
                StatusCode = 404
            };
        }

        // 🔹 **Lấy danh sách role của user**
        var roles = await _userManager.GetRolesAsync(user);

        // 🔹 **Kiểm tra nếu user là "Member"**
        if (roles.Contains(StaticUserRoles.Member))
        {
            user.FullName = updateUserProfileDto.FullName;
            user.AvatarUrl = updateUserProfileDto.AvatarUrl;
            user.Country = updateUserProfileDto.Country;
            user.CCCD = updateUserProfileDto.CCCD;
            user.Address = updateUserProfileDto.Address;
            user.BirthDate = updateUserProfileDto.BirthDate;
        }

        // 🔹 **Kiểm tra nếu user là "Organizer"**
        if (roles.Contains(StaticUserRoles.Organization))
        {
            user.OrganizationName = updateUserProfileDto.OrganizationName;
            user.TaxId = updateUserProfileDto.TaxId;
            user.AvatarUrl = updateUserProfileDto.AvatarUrl;
            user.Country = updateUserProfileDto.Country;
            user.Address = updateUserProfileDto.Address;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return new ResponseDto
            {
                Message = "Failed to update user profile",
                IsSuccess = false,
                StatusCode = 400,
                Result = updateResult.Errors
            };
        }

        string accessToken;
        if (roles.Contains(StaticUserRoles.Organization))
        {
            accessToken = await _tokenService.GenerateJwtAccessTokenOrganizationAsync(user);
        }
        else
        {
            accessToken = await _tokenService.GenerateJwtAccessTokenCustomerAsync(user);
        }

        var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
        await _tokenService.StoreRefreshToken(user.Id, refreshToken);

        return new ResponseDto
        {
            Result = new SignInResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            },
            Message = "User profile updated successfully",
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public Task<ResponseDto> GetUserById(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDto> FetchUserByToken(string token)
    {
        // Sử dụng GetPrincipalFromToken để lấy ClaimsPrincipal từ token
        var principal = await _tokenService.GetPrincipalFromToken(token);

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return new ResponseDto()
            {
                Message = "Invalid user",
                StatusCode = 400,
                IsSuccess = false,
                Result = null
            };
        }

        // Lấy role từ UserManager
        var roles = await _userManager.GetRolesAsync(user);

        GetUserDto userDto;

        if (roles.Contains(StaticUserRoles.Organization))
        {
            // Tạo GetUserDto từ claims
            userDto = new GetUserDto
            {
                Id = user.Id,
                OrganizationName = principal.FindFirst("OrganizationName")!.Value,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Address = principal.FindFirst("Address")?.Value,
                ImageUrl = principal.FindFirst("AvatarUrl")?.Value,
                TaxId = user.TaxId!,
                Roles = roles.ToList()
            };
        }
        else
        {
            // Tạo GetUserDto từ claims
            userDto = new GetUserDto
            {
                Id = user.Id,
                FullName = principal.FindFirst("FullName")!.Value,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Address = principal.FindFirst("Address")?.Value,
                ImageUrl = principal.FindFirst("AvatarUrl")?.Value,
                UserName = user.UserName!,
                CCCD = user.CCCD!,
                Roles = roles.ToList()
            };
        }

        return new ResponseDto()
        {
            Message = "Get info successfully",
            StatusCode = 200,
            IsSuccess = true,
            Result = userDto
        };
    }

    public async Task<ResponseDto> SendVerifyEmail(EmailDto emailDto)
    {
        // Tìm user theo email
        var user = await _userManager.FindByEmailAsync(emailDto.Email);
        if (user == null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "User not found",
                StatusCode = 404,
                Result = null
            };
        }

        // Nếu email đã được xác nhận
        if (user.EmailConfirmed)
        {
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Your email has already been confirmed",
                StatusCode = 200,
                Result = null
            };
        }

        // Sinh token xác nhận email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Xây dựng liên kết xác thực.
        // Lưu ý: thay đổi URL cho phù hợp với môi trường (local hay production)
        string verificationLink =
            $"http://localhost:5173/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        // Gọi EmailService để gửi email xác thực sử dụng template VerificationEmailTemplate
        bool emailSent = await _emailService.SendVerificationEmailAsync(user.Email!, verificationLink, user.FullName);

        if (emailSent)
        {
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Verification email sent successfully.",
                StatusCode = 200,
                Result = null
            };
        }
        else
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to send verification email.",
                StatusCode = 500,
                Result = null
            };
        }
    }

    public async Task<ResponseDto> VerifyEmail(VerifyEmailDto verifyEmailDto)
    {
        var user = await _userManager.FindByIdAsync(verifyEmailDto.UserId);

        if (user!.EmailConfirmed)
        {
            return new ResponseDto()
            {
                Message = "Your email has been confirmed!",
                IsSuccess = true,
                StatusCode = 200,
                Result = null
            };
        }

        string decodedToken = Uri.UnescapeDataString(verifyEmailDto.Token);

        var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!confirmResult.Succeeded)
        {
            return new()
            {
                Message = "Invalid token",
                StatusCode = 400,
                IsSuccess = false,
                Result = null
            };
        }

        return new()
        {
            Message = "Confirm Email Successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = null
        };
    }

    public async Task<ResponseDto> ChangePassword(ClaimsPrincipal userPrincipal, ChangePasswordDto changePasswordDto)
    {
        // Lấy UserId từ ClaimsPrincipal
        var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return new ResponseDto { IsSuccess = false, Message = "User not authenticated." };
        }

        // Lấy thông tin user từ UserManager
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ResponseDto { IsSuccess = false, Message = "User not found." };
        }

        // Không cho phép thay đổi mật khẩu trùng với mật khẩu cũ
        if (changePasswordDto.NewPassword == changePasswordDto.OldPassword)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "New password cannot be the same as the old password."
            };
        }

        // Thực hiện thay đổi mật khẩu
        var result =
            await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (result.Succeeded)
        {
            return new ResponseDto { IsSuccess = true, Message = "Password changed successfully." };
        }

        return new ResponseDto
        {
            IsSuccess = false,
            Message = "Password change failed. Please ensure the old password is correct."
        };
    }

    public async Task<ResponseDto> ForgotPassword(EmailDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

        if (user == null)
        {
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "If an account exists for this email, a password reset email has been sent.",
                StatusCode = 200,
                Result = null
            };
        }

        //token reset password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // build link reset password
        string resetLink =
            $"http://localhost:5173/reset-password?email={user.Email}&token={Uri.UnescapeDataString(token)}";

        bool emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink);

        if (emailSent)
        {
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Password reset email sent successfully.",
                StatusCode = 200,
                Result = null
            };
        }
        else
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to send password reset email.",
                StatusCode = 500,
                Result = null
            };
        }
    }

    public async Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        // Check if new password and confirm password match
        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "New password and confirmation password do not match.",
                StatusCode = 400,
                Result = null
            };
        }

        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "User not found",
                StatusCode = 404,
                Result = null
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Reset password failed",
                StatusCode = 400,
                Result = null
            };
        }
        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Password has been reset successfully.",
            StatusCode = 200,
            Result = null
        };
    }
}