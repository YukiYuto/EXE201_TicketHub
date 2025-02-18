using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
//using TicketHub.Utility.Validator;

namespace TicketHub.Models.DTO.Ticket;

public class UploadTicketImgDto
{
    [Required]
    //[MaxFileSize(1)]
    //[AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
    public IFormFile File { get; set; } = null!;
}