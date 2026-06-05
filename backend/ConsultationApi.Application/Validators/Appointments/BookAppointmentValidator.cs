using ConsultationApi.Application.DTOs.Appointments;
using FluentValidation;

namespace ConsultationApi.Application.Validators.Appointments;

public class BookAppointmentValidator
    : AbstractValidator<BookAppointmentDto>
{
    public BookAppointmentValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();

        RuleFor(x => x.SlotId)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}