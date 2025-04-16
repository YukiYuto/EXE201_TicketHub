using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Customer;
using TicketHub.Models.DTO.Orgainzer;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.Services.Service;

public class ManagerService : IManagerService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagerService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }


    public async Task<ResponseDto> GetRevenueProfit(DateTime startDate,
        DateTime endDate,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var transactions = await _unitOfWork.TransactionRepository.GetTransactionsAsync(startDate, endDate);
        var totalProfit = transactions.Sum(transaction => transaction.Amount);
        var pagedTransactions = transactions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        var totalPages = (int)Math.Ceiling((double)transactions.Count / pageSize);

        return new ResponseDto
        {
            Result = new
            {
                TotalProfit = totalProfit,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Transactions = pagedTransactions
            },
            Message = "Revenue profit retrieved successfully",
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> GetAllOrganizer(ClaimsPrincipal user,
        int pageNumber = 1,
        int pageSize = 10,
        string? filterOn = null,
        string? filterQuery = null,
        string? sortBy = null)
    {
        try
        {
            var isManager = user.IsInRole(StaticUserRoles.Manager);
            if (!isManager)
                return new ResponseDto
                {
                    Message = "Unauthorized access. Manager role required.",
                    IsSuccess = false,
                    StatusCode = 403,
                    Result = null
                };

            // Call the repository method
            var (organizers, totalCount) = await _unitOfWork.OrganizationRepository.GetOrganizerAsync(
                pageNumber,
                pageSize,
                filterOn,
                filterQuery,
                sortBy,
                isManager,
                nameof(ApplicationUser)
            );

            var organizerDtos = _mapper.Map<List<OrganizerGetAllDto>>(organizers);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new ResponseDto
            {
                Result = new
                {
                    Organizers = organizerDtos,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber
                },
                Message = "Organizers retrieved successfully",
                IsSuccess = true,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto
            {
                Message = $"An error occurred: {ex.Message}",
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }

    public async Task<ResponseDto> GetAllCustomer(ClaimsPrincipal user,
        int pageNumber = 1,
        int pageSize = 10,
        string? filterOn = null,
        string? filterQuery = null,
        string? sortBy = null)
    {
        try
        {
            var isManager = user.IsInRole(StaticUserRoles.Manager);
            if (!isManager)
                return new ResponseDto
                {
                    Message = "Unauthorized access. Manager role required.",
                    IsSuccess = false,
                    StatusCode = 403,
                    Result = null
                };

            // Call the repository method
            var (customers, totalCount) = await _unitOfWork.CustomerRepository.GetCustomerAsync(
                pageNumber,
                pageSize,
                filterOn,
                filterQuery,
                sortBy,
                isManager,
                nameof(ApplicationUser)
            );

            var customerDtos = _mapper.Map<List<CustomerGetAllDto>>(customers);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new ResponseDto
            {
                Result = new
                {
                    Customers = customerDtos,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber
                },
                Message = "Customers retrieved successfully",
                IsSuccess = true,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto
            {
                Message = $"An error occurred: {ex.Message}",
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
}