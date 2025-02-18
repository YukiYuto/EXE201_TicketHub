/*using TicketHub.Models.Domain;

namespace TicketHub.Utility.Validator;
using FluentValidation;

public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
{
    public ApplicationUserValidator()
    {
        RuleFor(user => user.UserType)
            .NotEmpty().WithMessage("UserType is required.");

        When(user => user.UserType == "Customer", () =>
        {
            RuleFor(user => user.CCCD)
                .NotEmpty().WithMessage("CCCD is required for customers.")
                .Length(12).WithMessage("CCCD must be 12 characters long.");
            RuleFor(user => user.BirthDate)
                .NotEmpty().WithMessage("BirthDate is required for customers.");
        });

        When(user => user.UserType == "Organizer", () =>
        {
            RuleFor(user => user.OrganizationName)
                .NotEmpty().WithMessage("Organization Name is required for organizers.");
            RuleFor(user => user.TaxId)
                .NotEmpty().WithMessage("Tax ID is required for organizers.");
        });
    }
}*/