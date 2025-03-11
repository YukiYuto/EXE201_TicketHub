using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Transaction;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class TransactionService : ITransactionService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<ResponseDto> GetAllTransactionByCustomerId
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
        if (customer == null)
            return new ResponseDto
            {
                Message = "Customer not found",
                IsSuccess = false,
                Result = null,
                StatusCode = 404
            };

        var transactions =
            await _unitOfWork.TransactionRepository.GetAllAsync(t => t.CustomerId == customer.CustomerId,
                "Payment");
        if (transactions.Any())
            return new ResponseDto
            {
                Message = "Transaction not found",
                IsSuccess = true,
                Result = transactions,
                StatusCode = 200
            };

        var responseDto = _mapper.Map<GetAllTransactionByCustomerIdDto>(transactions);

        return new ResponseDto
        {
            Message = "Get successful transaction history",
            IsSuccess = true,
            Result = responseDto,
            StatusCode = 200
        };
    }
}