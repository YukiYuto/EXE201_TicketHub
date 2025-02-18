using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Authentication;
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

    public AuthService
    (
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _tokenHandler = new JwtSecurityTokenHandler();
        _unitOfWork = unitOfWork;
        /*_mapperService = mapperService;
        _emailService = emailService;*/
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

    public Task<ResponseDto> GetUserById(Guid userId)
    {
        throw new NotImplementedException();
    }
}