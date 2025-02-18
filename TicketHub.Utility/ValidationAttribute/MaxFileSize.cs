using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TicketHub.Utility.ValidationAttribute;

public class MaxFileSize : System.ComponentModel.DataAnnotations.ValidationAttribute
{
    private readonly int _maxFileSize;

    public MaxFileSize(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            if (file.Length > (_maxFileSize * 2048 * 2048))
            {
                return new ValidationResult($"Maximum allowed file size is {_maxFileSize} MB.");
            }
        }

        return ValidationResult.Success;
    }
}