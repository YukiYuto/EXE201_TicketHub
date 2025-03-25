using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TicketHub.Utility.Validator;

namespace TicketHub.Models.DTO.Image;

public class AvatarUploadDto
{
    [Required]
    [MaxFileSize(1)]
    [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
    public IFormFile File { get; set; } = null!;
}