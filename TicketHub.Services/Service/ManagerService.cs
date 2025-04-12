using Microsoft.AspNetCore.Identity;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class ManagerService : IManagerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagerService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
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

    public Task<ResponseDto> GetRevenueCustomer(DateTime startDate,
        DateTime endDate,
        int pageNumber = 1,
        int pageSize = 10)
    {
        throw new NotImplementedException();
    }
}