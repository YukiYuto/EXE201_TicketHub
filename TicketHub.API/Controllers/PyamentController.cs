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

    [HttpGet("get-all")]
    public async Task<ActionResult<ResponseDto>> GetAll
    (
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterQuery = null,
        [FromQuery] string? filterOn = null,
        [FromQuery] string? sortBy = null
    )
    {
        var responseDto = await _paymentService.GetAll(User, pageNumber, pageSize, filterQuery, filterOn, sortBy);

        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("get-payment-link/{paymentTransactionId}")]
    public async Task<ActionResult<ResponseDto>> GetPaymentLink(Guid paymentTransactionId)
    {
        var responseDto = await _paymentService.GetPaymentLink(User, paymentTransactionId);

        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPost("create-payment-link")]
    public async Task<ActionResult<ResponseDto>> CreatePaymentLink([FromBody] CreatePaymentLinkDto createPaymentLinkDTO)
    {
        var responseDto = await _paymentService.CreatePayOsPaymentLink(User, createPaymentLinkDTO);

        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPost("confirm-payment")]
    public async Task<ActionResult<ResponseDto>> ConfirmPayment([FromBody] ConfirmPayment confirmPaymentDTO)
    {
        var responseDto = await _paymentService.ConfirmPayOsTransaction(confirmPaymentDTO);

        return StatusCode(responseDto.StatusCode, responseDto);
    }
}