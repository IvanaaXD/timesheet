using FluentValidation;
using TimeSheet.Application.DTOs.Country;

namespace TimeSheet.Application.Validators
{
    public class CountryDTOValidator : AbstractValidator<CountryRequestDTO>
    {
        public CountryDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");
        }
    }
}