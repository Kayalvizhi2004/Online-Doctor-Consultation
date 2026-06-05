using ConsultationApi.Application.DTOs.Chat;
using FluentValidation;

namespace ConsultationApi.Application.Validators.Chat;

public class SendMessageValidator
    : AbstractValidator<SendMessageDto>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(2000);
    }
}