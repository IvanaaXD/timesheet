using FluentValidation;
using TimeSheet.Application.DTOs.Category; 

namespace TimeSheet.Application.Validators
{
    public class CategoryDTOValidator : AbstractValidator<CategoryRequestDTO>
    {
        public CategoryDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");
        }
    }
}