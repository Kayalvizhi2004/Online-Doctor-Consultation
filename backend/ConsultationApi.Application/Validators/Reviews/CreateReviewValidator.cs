using ConsultationApi.Application.DTOs.Reviews;
using FluentValidation;

namespace ConsultationApi.Application.Validators.Reviews;

public class CreateReviewValidator
    : AbstractValidator<CreateReviewDto>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.Comment)
            .NotEmpty()
            .MaximumLength(1000);
    }
}