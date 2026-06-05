using ConsultationApi.Application.DTOs.Doctors;
using FluentValidation;

namespace ConsultationApi.Application.Validators.Doctors;

public class DoctorProfileUpdateValidator
    : AbstractValidator<DoctorProfileUpdateDto>
{
    public DoctorProfileUpdateValidator()
    {
        RuleFor(x => x.Bio)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.ConsultationFee)
            .GreaterThan(0)
            .LessThanOrEqualTo(10000);
    }
}