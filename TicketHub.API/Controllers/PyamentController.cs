using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Payment;
using TicketHub.Services.IService;

namespace TicketHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-payment-link")]
    public async Task<ActionResult<ResponseDto>> CreatePaymentLink([FromBody] CreatePaymentLinkDto createPaymentLinkDTO)
    {
        var responseDto = await _paymentService.CreatePayOsPaymentLink(User, createPaymentLinkDTO);

        return StatusCode(responseDto.StatusCode, responseDto);
    }
}