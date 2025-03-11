using Microsoft.AspNetCore.Mvc;
using TicketHub.Services.IService;

namespace TicketHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("GetAllTransactionByCustomerId")]
    public async Task<IActionResult> GetAllTransactionByCustomerId
    (
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var response = await _transactionService.GetAllTransactionByCustomerId
        (
            User,
            filterOn,
            filterQuery,
            sortBy,
            pageNumber,
            pageSize
        );
        return StatusCode(response.StatusCode, response);
    }
}