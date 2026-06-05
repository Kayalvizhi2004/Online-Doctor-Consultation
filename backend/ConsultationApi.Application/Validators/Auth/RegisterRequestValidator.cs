using ConsultationApi.Application.DTOs.Auth;
using FluentValidation;

namespace ConsultationApi.Application.Validators.Auth;

public class RegisterRequestValidator
    : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]")
            .WithMessage(
                "Password must contain uppercase letter")
            .Matches(@"[a-z]")
            .WithMessage(
                "Password must contain lowercase letter")
            .Matches(@"[0-9]")
            .WithMessage(
                "Password must contain number");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^[0-9]{10}$")
            .WithMessage(
                "Phone must be 10 digits");

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role =>
                role == "Patient" ||
                role == "Doctor")
            .WithMessage(
                "Role must be Patient or Doctor");
    }
}