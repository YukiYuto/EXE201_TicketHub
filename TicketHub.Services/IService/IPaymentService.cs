using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Payment;

namespace TicketHub.Services.IService;

public interface IPaymentService
{
    Task<ResponseDto> CreatePayOsPaymentLink(ClaimsPrincipal User, CreatePaymentLinkDto createPaymentLink);
    Task<ResponseDto> ConfirmPayOsTransaction(ConfirmPayment confirmPayment);
    Task<ResponseDto> CancelPayOsPaymentLink(ClaimsPrincipal User, Guid paymentTransactionId, string cancellationReason);
}