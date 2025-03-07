using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;

namespace TicketHub.API.Controllers;

public class UploadImageController : Controller
{
    private readonly IFirebaseService _firebaseService;

    public UploadImageController(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, string imageType, string? oldImageUrl = null)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ResponseDto { IsSuccess = false, StatusCode = 400, Message = "Invalid file!" });

        // 🔹 Tạo thư mục dựa trên imageType
        var folder = imageType switch
        {
            "user" => "UserAvatars",
            "ticket" => "TicketImages",
            "event" => "EventImages",
            "resale" => "ResaleListings",
            _ => "OtherImages" // Mặc định nếu không khớp
        };

        var response = await _firebaseService.UploadImage(file, folder, oldImageUrl);

        if (!response.IsSuccess)
            return BadRequest(response);

        return StatusCode(response.StatusCode, response);
    }
}