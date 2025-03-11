using System.Security.Claims;
using TicketHub.Models.DTO;

namespace TicketHub.Services.IService;

public interface ITransactionService
{
    Task<ResponseDto> GetAllTransactionByCustomerId
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10
    );
}