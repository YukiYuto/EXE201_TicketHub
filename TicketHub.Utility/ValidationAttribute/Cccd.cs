namespace TicketHub.Utility.ValidationAttribute
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class Cccd : ValidationAttribute
    {
        public Cccd()
        {
            ErrorMessage = "CCCD must be exactly 12 digits.";
        }

        public override bool IsValid(object? value)
        {
            if (value is string cccd)
            {
                // Kiểm tra nếu CCCD là 12 chữ số
                return Regex.IsMatch(cccd, @"^\d{12}$");
            }
            return false;
        }
    }
}
