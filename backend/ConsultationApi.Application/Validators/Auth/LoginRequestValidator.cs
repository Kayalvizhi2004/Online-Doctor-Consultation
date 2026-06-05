using ConsultationApi.Application.DTOs.Auth;
using FluentValidation;

namespace ConsultationApi.Application.Validators.Auth;

public class LoginRequestValidator
    : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}