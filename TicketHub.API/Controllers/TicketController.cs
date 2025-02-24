using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Ticket;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetTickets(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var responseDto = await _ticketService.GetTickets(
                User, filterOn, filterQuery, sortBy, pageNumber, pageSize
            );
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpGet("{ticketId}")]
        public async Task<ActionResult<ResponseDto>> GetTicket([FromRoute] Guid ticketId)
        {
            var responseDto = await _ticketService.GetTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpGet("user")]
        public async Task<ActionResult<ResponseDto>> GetTicketByUserId()
        {
            var responseDto = await _ticketService.GetTicketByUserId(User);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Authorize]
        [Route("organization")]
        public async Task<ActionResult<ResponseDto>> CreateTicketByOrganization(
            [FromBody] List<CreateTicketDto> createTicketDtos)
        {
            var responseDto = await _ticketService.CreateTicketByOrganiztion(User, createTicketDtos);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Authorize]
        [Route("customer")]
        public async Task<ActionResult<ResponseDto>> CreateTicketByCustomer(
            [FromBody] CreateTicketDto createTicketDto)
        {
            var responseDto = await _ticketService.CreateTicketByCustomer(User, createTicketDto);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> UpdateTicket(
            [FromBody] UpdateTicketDto updateTicketDto)
        {
            var responseDto = await _ticketService.UpdateTicket(User, updateTicketDto);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpDelete("{ticketId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> DeleteTicket([FromRoute] Guid ticketId)
        {
            var responseDto = await _ticketService.DeleteTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Route("upload-image")]
        public async Task<ActionResult<ResponseDto>> UploadTicketImage
        (
            UploadTicketImgDto uploadTicketImgDto
        )
        {
            var responseDto =
                await _ticketService.UploadTicketImage
                (
                    User,
                    uploadTicketImgDto
                );
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Authorize(Roles = StaticUserRoles.Staff)]
        [Route("accept/{ticketId}")]
        public async Task<ActionResult<ResponseDto>> AcceptTicket([FromRoute] Guid ticketId)
        {
            var responseDto = await _ticketService.AcceptTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Authorize(Roles = StaticUserRoles.Staff)]
        [Route("reject/{ticketId}")]
        public async Task<ActionResult<ResponseDto>> RejectTicket([FromRoute] Guid ticketId)
        {
            var responseDto = await _ticketService.RejectTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }
    }
}