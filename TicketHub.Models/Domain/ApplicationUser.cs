using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TicketHub.Models.Domain;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)] public string? FullName { get; set; }

    [StringLength(500)] public string? AvatarUrl { get; set; }

    [StringLength(50)] public string? Country { get; set; }

    [StringLength(100)] public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
}