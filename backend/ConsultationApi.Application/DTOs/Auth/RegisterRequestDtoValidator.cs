using FluentValidation;

namespace ConsultationApi.Application.DTOs.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        // FullName validation
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters.");

        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 100).WithMessage("Password must be at least 6 characters long.");

        // Phone validation
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format."); 
            // Note: FluentValidation's built-in email/phone checks work best 
            // when paired with a clean regex format match like the one above.

        // Role validation
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.");

        // Specialization validation
        RuleFor(x => x.Specialization)
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");
    }
}
