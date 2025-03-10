using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Payment;

namespace TicketHub.Services.IService;

public interface IPaymentService
{
    Task<ResponseDto> CreatePayOsPaymentLink(ClaimsPrincipal User, CreatePaymentLinkDto createPaymentLink);

    Task<ResponseDto> ConfirmPayOsTransaction(ConfirmPayment confirmPayment);

    Task<ResponseDto> GetPaymentLink(ClaimsPrincipal User, Guid paymentTransactionId);

    Task<ResponseDto> GetAll
    (
        ClaimsPrincipal User,
        int pageNumber = 1,
        int pageSize = 10,
        string? filterQuery = null,
        string? filterOn = null,
        string? sortBy = null
    );
}