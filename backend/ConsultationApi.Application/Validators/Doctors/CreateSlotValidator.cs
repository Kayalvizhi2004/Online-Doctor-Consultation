using ConsultationApi.Application.DTOs.Doctors;
using FluentValidation;

namespace ConsultationApi.Application.Validators.Doctors;

public class CreateSlotValidator
    : AbstractValidator<CreateSlotDto>
{
    public CreateSlotValidator()
    {
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(
                DateOnly.FromDateTime(DateTime.Today))
            .WithMessage(
                "Date cannot be past");

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage(
                "EndTime must be after StartTime");
    }
}