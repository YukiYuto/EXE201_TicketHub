using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Authentication;
using TicketHub.Models.DTO.Authentication.Google;
using TicketHub.Models.DTO.Email;
using TicketHub.Models.DTO.Image;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.Services.Service;

public class AuthService : IAuthService
{
    private readonly IEmailService _emailService;
    private readonly IFirebaseService _firebaseService;
    private readonly IRedisService _redisService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService
    (
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IRedisService redisService,
        IFirebaseService firebaseService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _tokenHandler = new JwtSecurityTokenHandler();
        _unitOfWork = unitOfWork;
        /*_mapperService = mapperService;*/
        _emailService = emailService;
        _redisService = redisService;
        _firebaseService = firebaseService;
    }

    public async Task<ResponseDto> SignUpCustomer(SignUpCustomerDto signUpCustomerDto)
    {
        // Kiểm tra email đã tồn tại
        var isEmailExit = await _userManager.FindByEmailAsync(signUpCustomerDto.Email);
        if (isEmailExit is not null)
            return new ResponseDto
            {
                Message = "Email is being used by another user",
                Result = signUpCustomerDto,
                IsSuccess = false,
                StatusCode = 400
            };

        // Kiểm tra số điện thoại đã tồn tại
        var isPhoneNumberExit = await _userManager.Users
            .AnyAsync(u => u.PhoneNumber == signUpCustomerDto.PhoneNumber);
        if (isPhoneNumberExit)
            return new ResponseDto
            {
                Message = "Phone number is being used by another user",
                Result = signUpCustomerDto,
                IsSuccess = false,
                StatusCode = 400
            };

        // Tạo đối tượng ApplicationUser mới
        var newUser = new ApplicationUser
        {
            Email = signUpCustomerDto.Email,
            UserName = signUpCustomerDto.Email,
            FullName = signUpCustomerDto.FullName,
            Address = signUpCustomerDto.Address,
            Country = signUpCustomerDto.Country,
            //BirthDate = signUpCustomerDto.BirthDate,
            PhoneNumber = signUpCustomerDto.PhoneNumber,
            //CCCD = signUpCustomerDto.CCCD,
            AvatarUrl = "",
            LockoutEnabled = false
        };

        // Thêm người dùng mới vào database
        var createUserResult = await _userManager.CreateAsync(newUser, signUpCustomerDto.Password);

        // Kiểm tra lỗi khi tạo
        if (!createUserResult.Succeeded)
            return new ResponseDto
            {
                Message = "Create user failed",
                IsSuccess = false,
                StatusCode = 400,
                Result = signUpCustomerDto
            };

        var user = newUser;
        var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.Member);

        if (!isRoleExist) await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Member));


        // Thêm role "Customer" cho người dùng
        var isRoleAdded = await _userManager.AddToRoleAsync(newUser, StaticUserRoles.Member);

        if (!isRoleAdded.Succeeded)
            return new ResponseDto
            {
                Message = "Error adding role",
                IsSuccess = false,
                StatusCode = 500,
                Result = signUpCustomerDto
            };

        await _unitOfWork.SaveAsync();

        return new ResponseDto
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
            return new ResponseDto
            {
                Message = "Email is being used by another user",
                Result = signUpOrganizationDto,
                IsSuccess = false,
                StatusCode = 400
            };

        // Kiểm tra số điện thoại đã tồn tại
        var isPhoneNumberExit = await _userManager.Users
            .AnyAsync(u => u.PhoneNumber == signUpOrganizationDto.PhoneNumber);
        if (isPhoneNumberExit)
            return new ResponseDto
            {
                Message = "Phone number is being used by another user",
                Result = signUpOrganizationDto,
                IsSuccess = false,
                StatusCode = 400
            };

        // Tạo đối tượng ApplicationUser mới
        var newUser = new ApplicationUser
        {
            Email = signUpOrganizationDto.Email,
            UserName = signUpOrganizationDto.Email,
            //OrganizationName = signUpOrganizationDto.OrganizationName,
            Address = signUpOrganizationDto.Address,
            Country = signUpOrganizationDto.Country,
            //TaxId = signUpOrganizationDto.TaxId,
            PhoneNumber = signUpOrganizationDto.PhoneNumber,
            AvatarUrl = "",
            LockoutEnabled = false
        };

        // Thêm người dùng mới vào database
        var createUserResult = await _userManager.CreateAsync(newUser, signUpOrganizationDto.Password);

        // Kiểm tra lỗi khi tạo
        if (!createUserResult.Succeeded)
            return new ResponseDto
            {
                Message = "Create user failed",
                IsSuccess = false,
                StatusCode = 400,
                Result = signUpOrganizationDto
            };

        var user = newUser;
        var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.Organization);

        if (!isRoleExist) await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Organization));


        // Thêm role "Customer" cho người dùng
        var isRoleAdded = await _userManager.AddToRoleAsync(newUser, StaticUserRoles.Organization);

        if (!isRoleAdded.Succeeded)
            return new ResponseDto
            {
                Message = "Error adding role",
                IsSuccess = false,
                StatusCode = 500,
                Result = signUpOrganizationDto
            };

        await _unitOfWork.SaveAsync();

        return new ResponseDto
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
            return new ResponseDto
            {
                Message = "User does not exist!",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, signDto.Password);

        if (!isPasswordCorrect)
            return new ResponseDto
            {
                Message = "Incorrect email or password",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };

        var role = await _userManager.GetRolesAsync(user);

        if (!user.EmailConfirmed)
            return new ResponseDto
            {
                Message = "You need to confirm email!",
                Result = null,
                IsSuccess = false,
                StatusCode = 401
            };

        if (user.LockoutEnd is not null)
            return new ResponseDto
            {
                Message = "User has been locked",
                IsSuccess = false,
                StatusCode = 403,
                Result = null
            };

        string accessToken;
        if (role.Contains(StaticUserRoles.Organization))
            accessToken = await _tokenService.GenerateJwtAccessTokenOrganizationAsync(user);
        else
            accessToken = await _tokenService.GenerateJwtAccessTokenCustomerAsync(user);

        var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
        await _tokenService.StoreRefreshToken(user.Id, refreshToken);

        return new ResponseDto
        {
            Result = new SignInResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            },
            Message = "Sign in successfully",
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> SignInByGoogle(SignInByGoogleDto signInByGoogleDto)
    {
        // Gọi API của Google để lấy thông tin từ Access Token
        var httpClient = new HttpClient();
        var response =
            await httpClient.GetStringAsync(
                $"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={signInByGoogleDto.TokenGoogle}");

        // Parse response từ Google
        var googleUser = JsonConvert.DeserializeObject<GoogleUserInfo>(response);
        if (googleUser == null || googleUser.email == null)
            return new ResponseDto
            {
                Message = "Invalid Google Access Token",
                IsSuccess = false,
                StatusCode = 401
            };

        var email = googleUser.email;

        // Tìm kiếm người dùng trong database
        var user = await _userManager.FindByEmailAsync(email);
        UserLoginInfo? userLoginInfo = null;
        if (user is not null)
            userLoginInfo = (await _userManager.GetLoginsAsync(user))
                .FirstOrDefault(x => x.LoginProvider == StaticLoginProvider.Google);

        if (user?.LockoutEnd is not null)
            return new ResponseDto
            {
                Message = "User has been locked",
                IsSuccess = false,
                StatusCode = 403,
                Result = null
            };

        if (user is not null && userLoginInfo is null)
            return new ResponseDto
            {
                Result = new SignResponseDto
                {
                    AccessToken = "",
                    RefreshToken = ""
                },
                Message = "The email is using by another user",
                IsSuccess = false,
                StatusCode = 400
            };

        // Nếu user chưa tồn tại, tạo user mới và thêm role "Member"
        if (user is null)
        {
            user = new ApplicationUser
            {
                Email = email,
                FullName = "",
                UserName = email,
                AvatarUrl = "",
                Country = "",
                //CCCD = "",
                Address = "",
                EmailConfirmed = true
            };

            // Tạo user mới trong database
            var createUserResult = await _userManager.CreateAsync(user);
            if (!createUserResult.Succeeded)
                return new ResponseDto
                {
                    Message = "Error creating user",
                    IsSuccess = false,
                    StatusCode = 400
                };

            // Thêm thông tin đăng nhập Google vào tài khoản
            await _userManager.AddLoginAsync(user,
                new UserLoginInfo(StaticLoginProvider.Google, googleUser.sub, "GOOGLE"));

            // Kiểm tra và tạo role "Member" nếu chưa có
            var isRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.Member);
            if (!isRoleExist) await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Member));

            // Thêm role "Member" cho người dùng mới
            var isRoleAdded = await _userManager.AddToRoleAsync(user, StaticUserRoles.Member);
            if (!isRoleAdded.Succeeded)
                return new ResponseDto
                {
                    Message = "Error adding role",
                    IsSuccess = false,
                    StatusCode = 500
                };
        }

        // Cập nhật thông tin người dùng
        await _userManager.UpdateAsync(user);

        // Kiểm tra thông tin bắt buộc đã được cập nhật chưa
        var isProfileComplete =
            !string.IsNullOrEmpty(user.FullName) &&
            !string.IsNullOrEmpty(user.Address) &&
            !string.IsNullOrEmpty(user.AvatarUrl) &&
            !string.IsNullOrEmpty(user.Country);
        //!string.IsNullOrEmpty(user.CCCD);

        // Tạo Access Token và Refresh Token cho user
        var accessToken = await _tokenService.GenerateJwtAccessTokenCustomerAsync(user!);
        var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user!);
        await _tokenService.StoreRefreshToken(user!.Id, refreshToken);

        // Nếu hồ sơ chưa hoàn chỉnh, trả về cảnh báo
        if (!isProfileComplete)
            return new ResponseDto
            {
                Result = new SignByGoogleResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    IsProfileComplete = false
                },
                Message = "Your profile is incomplete. Please update your profile information.",
                IsSuccess = true,
                StatusCode = 200
            };

        // Nếu thông tin đầy đủ
        return new ResponseDto
        {
            Result = new SignByGoogleResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsProfileComplete = true
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
            return new ResponseDto
            {
                Message = "Unauthorized",
                StatusCode = 401,
                IsSuccess = false,
                Result = null
            };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new ResponseDto
            {
                Message = "User not found",
                IsSuccess = false,
                StatusCode = 404
            };

        // 🔹 **Lấy danh sách role của user**
        var roles = await _userManager.GetRolesAsync(user);

        // 🔹 **Kiểm tra nếu user là "Member"**
        if (roles.Contains(StaticUserRoles.Member))
        {
            user.FullName = updateUserProfileDto.FullName;
            user.AvatarUrl = updateUserProfileDto.AvatarUrl;
            user.Country = updateUserProfileDto.Country;
            //user.CCCD = updateUserProfileDto.CCCD;
            user.Address = updateUserProfileDto.Address;
            //user.BirthDate = updateUserProfileDto.BirthDate;
            user.AvatarUrl = updateUserProfileDto.AvatarUrl;
        }

        // 🔹 **Kiểm tra nếu user là "Organizer"**
        if (roles.Contains(StaticUserRoles.Organization))
        {
            //user.OrganizationName = updateUserProfileDto.OrganizationName;
            //user.TaxId = updateUserProfileDto.TaxId;
            user.AvatarUrl = updateUserProfileDto.AvatarUrl;
            user.Country = updateUserProfileDto.Country;
            user.Address = updateUserProfileDto.Address;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return new ResponseDto
            {
                Message = "Failed to update user profile",
                IsSuccess = false,
                StatusCode = 400,
                Result = updateResult.Errors
            };

        string accessToken;
        if (roles.Contains(StaticUserRoles.Organization))
            accessToken = await _tokenService.GenerateJwtAccessTokenOrganizationAsync(user);
        else
            accessToken = await _tokenService.GenerateJwtAccessTokenCustomerAsync(user);

        var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
        await _tokenService.StoreRefreshToken(user.Id, refreshToken);

        return new ResponseDto
        {
            Result = new SignInResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            },
            Message = "User profile updated successfully",
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        try
        {
            if (refreshTokenDto == null || string.IsNullOrEmpty(refreshTokenDto.RefreshToken))
                return new ResponseDto
                {
                    Message = "Invalid refresh token",
                    IsSuccess = false,
                    StatusCode = 400
                };

            // Giải mã token để lấy userId
            var principal = await _tokenService.GetPrincipalFromToken(refreshTokenDto.RefreshToken);
            var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return new ResponseDto
                {
                    Message = "Invalid token",
                    IsSuccess = false,
                    StatusCode = 400
                };

            // Kiểm tra refresh token có tồn tại trên Redis không
            var redisKey = $"userId:{userId}:refreshToken";
            var storedRefreshToken = await _redisService.RetrieveString(redisKey);

            if (string.IsNullOrEmpty(storedRefreshToken) || storedRefreshToken != refreshTokenDto.RefreshToken)
                return new ResponseDto
                {
                    Message = "Refresh token expired or invalid. Please log in again.",
                    IsSuccess = false,
                    StatusCode = 401
                };

            // Lấy thông tin user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseDto
                {
                    Message = "User not found",
                    IsSuccess = false,
                    StatusCode = 404
                };

            // Cấp mới access token
            var newAccessToken = await _tokenService.GenerateJwtAccessTokenCustomerAsync(user);

            return new ResponseDto
            {
                Message = "Access token refreshed successfully",
                IsSuccess = true,
                StatusCode = 200,
                Result = newAccessToken
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto
            {
                Message = "An error occurred: " + ex.Message,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }

    public async Task<ResponseDto> FetchUserByToken(ClaimsPrincipal user)
    {
        try
        {
            // Lấy userId từ ClaimsPrincipal
            var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new ResponseDto
                {
                    Message = "Invalid user",
                    StatusCode = 401,
                    IsSuccess = false,
                    Result = null
                };

            // Tìm user trong database
            var userEntity = await _userManager.FindByIdAsync(userId);
            if (userEntity == null)
                return new ResponseDto
                {
                    Message = "User not found",
                    StatusCode = 404,
                    IsSuccess = false,
                    Result = null
                };

            // Lấy danh sách vai trò của user
            var roles = await _userManager.GetRolesAsync(userEntity);

            GetUserDto userDto;

            if (roles.Contains(StaticUserRoles.Organization))
                // Tạo DTO cho Organization
                userDto = new GetUserDto
                {
                    Id = userEntity.Id,
                    OrganizationName = user.Claims.FirstOrDefault(x => x.Type == "OrganizationName")?.Value,
                    Email = userEntity.Email!,
                    PhoneNumber = userEntity.PhoneNumber!,
                    Address = user.Claims.FirstOrDefault(x => x.Type == "Address")?.Value,
                    ImageUrl = user.Claims.FirstOrDefault(x => x.Type == "AvatarUrl")?.Value,
                    //TaxId = userEntity.TaxId!,
                    Roles = roles.ToList()
                };
            else
                // Tạo DTO cho User thông thường
                userDto = new GetUserDto
                {
                    Id = userEntity.Id,
                    FullName = user.Claims.FirstOrDefault(x => x.Type == "FullName")?.Value,
                    Email = userEntity.Email!,
                    PhoneNumber = userEntity.PhoneNumber!,
                    Address = user.Claims.FirstOrDefault(x => x.Type == "Address")?.Value,
                    ImageUrl = user.Claims.FirstOrDefault(x => x.Type == "AvatarUrl")?.Value,
                    UserName = userEntity.UserName!,
                    //CCCD = userEntity.CCCD!,
                    Roles = roles.ToList()
                };

            return new ResponseDto
            {
                Message = "Get user info successfully",
                StatusCode = 200,
                IsSuccess = true,
                Result = userDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = "An error occurred: " + e.Message,
                StatusCode = 500,
                IsSuccess = false,
                Result = null
            };
        }
    }

    public async Task<ResponseDto> SendVerifyEmail(EmailDto emailDto)
    {
        // Tìm user theo email
        var user = await _userManager.FindByEmailAsync(emailDto.Email);
        if (user == null)
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "User not found",
                StatusCode = 404,
                Result = null
            };

        // Nếu email đã được xác nhận
        if (user.EmailConfirmed)
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Your email has already been confirmed",
                StatusCode = 200,
                Result = null
            };

        // Sinh token xác nhận email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Xây dựng liên kết xác thực.
        // Lưu ý: thay đổi URL cho phù hợp với môi trường (local hay production)
        var verificationLink =
            $"http://localhost:5173/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        // Gọi EmailService để gửi email xác thực sử dụng template VerificationEmailTemplate
        var emailSent = await _emailService.SendVerificationEmailAsync(user.Email!, verificationLink, user.FullName);

        if (emailSent)
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Verification email sent successfully.",
                StatusCode = 200,
                Result = null
            };

        return new ResponseDto
        {
            IsSuccess = false,
            Message = "Failed to send verification email.",
            StatusCode = 500,
            Result = null
        };
    }

    public async Task<ResponseDto> VerifyEmail(VerifyEmailDto verifyEmailDto)
    {
        var user = await _userManager.FindByIdAsync(verifyEmailDto.UserId);

        if (user!.EmailConfirmed)
            return new ResponseDto
            {
                Message = "Your email has been confirmed!",
                IsSuccess = true,
                StatusCode = 200,
                Result = null
            };

        var decodedToken = Uri.UnescapeDataString(verifyEmailDto.Token);

        var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!confirmResult.Succeeded)
            return new ResponseDto
            {
                Message = "Invalid token",
                StatusCode = 400,
                IsSuccess = false,
                Result = null
            };

        return new ResponseDto
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
            return new ResponseDto { IsSuccess = false, Message = "User not authenticated." };

        // Lấy thông tin user từ UserManager
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new ResponseDto { IsSuccess = false, Message = "User not found." };

        // Không cho phép thay đổi mật khẩu trùng với mật khẩu cũ
        if (changePasswordDto.NewPassword == changePasswordDto.OldPassword)
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "New password cannot be the same as the old password."
            };

        // Thực hiện thay đổi mật khẩu
        var result =
            await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (result.Succeeded) return new ResponseDto { IsSuccess = true, Message = "Password changed successfully." };

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
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "If an account exists for this email, a password reset email has been sent.",
                StatusCode = 200,
                Result = null
            };

        //token reset password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // 🔹 Mã hóa token bằng Base64 trước khi đưa vào URL
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

        // Tạo link reset password
        var resetLink = $"http://localhost:5173/reset-password?email={user.Email}&token={encodedToken}";

        var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink);

        if (emailSent)
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Password reset email sent successfully.",
                StatusCode = 200,
                Result = null
            };

        return new ResponseDto
        {
            IsSuccess = false,
            Message = "Failed to send password reset email.",
            StatusCode = 500,
            Result = null
        };
    }

    public async Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(resetPasswordDto.Token));
        }
        catch (Exception)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid or corrupted token.",
                StatusCode = 400,
                Result = null
            };
        }

        // Check if new password and confirm password match
        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "New password and confirmation password do not match.",
                StatusCode = 400,
                Result = null
            };

        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "User not found",
                StatusCode = 404,
                Result = null
            };

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
        if (!result.Succeeded)
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Reset password failed",
                StatusCode = 400,
                Result = null
            };

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Password has been reset successfully.",
            StatusCode = 200,
            Result = null
        };
    }

    public async Task<ResponseDto> UploadUserAvatar(IFormFile file, ClaimsPrincipal User)
    {
        try
        {
            if (file == null || file.Length == 0)
                return new ResponseDto
                {
                    Message = "File is empty!",
                    IsSuccess = false,
                    StatusCode = 400
                };

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new ResponseDto
                {
                    Message = "Not authenticated!",
                    IsSuccess = false,
                    StatusCode = 401
                };

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseDto
                {
                    Message = "User does not exist",
                    IsSuccess = false,
                    StatusCode = 404
                };

            // 🔹 Upload ảnh lên Firebase
            var responseDto = await _firebaseService.UploadImageUser(file, StaticFirebaseFolders.UserAvatars);
            if (!responseDto.IsSuccess || string.IsNullOrEmpty(responseDto.Result?.ToString()))
                return new ResponseDto
                {
                    Message = "Image upload failed!",
                    IsSuccess = false,
                    StatusCode = 500
                };

            // 🔹 Cập nhật Avatar URL
            user.AvatarUrl = responseDto.Result?.ToString();
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new ResponseDto
                {
                    Message = "Failed to update user avatar!",
                    IsSuccess = false,
                    StatusCode = 500
                };

            // 🔹 Xóa refresh token cũ nếu có
            var existingRefreshToken = await _tokenService.RetrieveRefreshToken(user.Id);
            if (!string.IsNullOrEmpty(existingRefreshToken)) await _tokenService.DeleteRefreshToken(user.Id);

            // 🔹 Tạo Access Token & Refresh Token mới
            var accessToken = await _tokenService.GenerateJwtAccessTokenCustomerAsync(user);
            var refreshToken = await _tokenService.GenerateJwtRefreshTokenAsync(user);
            await _tokenService.StoreRefreshToken(user.Id, refreshToken);

            return new ResponseDto
            {
                Message = "Upload user avatar successfully!",
                IsSuccess = true,
                StatusCode = 200,
                Result = new AvatarTokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = $"Error: {e.Message}",
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
}