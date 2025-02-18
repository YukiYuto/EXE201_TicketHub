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

        /// <summary>
        /// Get all tickets
        /// </summary>
        /// <param name="filterOn"></param>
        /// <param name="filterQuery"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetTickets
        (
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var responseDto = await _ticketService.GetTickets
            (
                User,
                filterOn,
                filterQuery,
                sortBy,
                pageNumber,
                pageSize
            );
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        /// <summary>
        /// Get ticket by ticketId
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ticketId}")]
        public async Task<ActionResult<ResponseDto>> GetTicket
        (
            [FromRoute] Guid ticketId
        )
        {
            var responseDto = await _ticketService.GetTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpGet]
        [Route("user")]
        public async Task<ActionResult<ResponseDto>> GetTicketByUserId()
        {
            var responseDto = await _ticketService.GetTicketByUserId(User);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        /// <summary>
        /// Create a new ticket
        /// </summary>
        /// <param name="createLevelDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> CreateTicket
        (
            [FromBody] List<CreateTicketDto> createTicketDtos
        )
        {
            var responseDto = await _ticketService.CreateTicket(User, createTicketDtos);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        /// <summary>
        /// Update ticket
        /// </summary>
        /// <param name="updateLevelDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> UpdateTicket
        (
            [FromBody] UpdateTicketDto updateLevelDto
        )
        {
            var responseDto = await _ticketService.UpdateTicket(User, updateLevelDto);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        /// <summary>
        /// Delete ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpDelete("{ticketId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> DeleteTicket
        (
            [FromRoute] Guid ticketId
        )
        {
            var responseDto = await _ticketService.DeleteTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="uploadTicketImgDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Accept a ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpPost("{ticketId}/accept")]
        [Authorize(Roles = StaticUserRoles.Staff)]
        public async Task<ActionResult<ResponseDto>> AcceptTicket([FromRoute] Guid ticketId)
        {
            var responseDto = await _ticketService.AcceptTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        /// <summary>
        /// Reject a ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpPost("{ticketId}/reject")]
        [Authorize(Roles = StaticUserRoles.Staff)]
        public async Task<ActionResult<ResponseDto>> RejectTicket([FromRoute] Guid ticketId)
        {
            var responseDto = await _ticketService.RejectTicket(User, ticketId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }
    }
}