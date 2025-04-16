using System.Security.Claims;
using TicketHub.Models.DTO;

namespace TicketHub.Services.IService;

public interface IManagerService
{
    Task<ResponseDto> GetRevenueProfit(
        DateTime startDate,
        DateTime endDate,
        int pageNumber = 1,
        int pageSize = 10
    );

    Task<ResponseDto> GetAllOrganizer(
        ClaimsPrincipal user,
        int pageNumber = 1,
        int pageSize = 10,
        string? filterOn = null,
        string? filterQuery = null,
        string? sortBy = null
    );

    Task<ResponseDto> GetAllCustomer(
        ClaimsPrincipal user,
        int pageNumber = 1,
        int pageSize = 10,
        string? filterOn = null,
        string? filterQuery = null,
        string? sortBy = null
    );
}