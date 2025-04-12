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

    Task<ResponseDto> GetRevenueCustomer(
        DateTime startDate,
        DateTime endDate,
        int pageNumber = 1,
        int pageSize = 10
    );
}