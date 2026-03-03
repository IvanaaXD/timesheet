using FluentValidation;
using TimeSheet.Application.DTOs.Project; 

namespace TimeSheet.Application.Validators
{
    public class ProjectDTOValidator : AbstractValidator<ProjectRequestDTO>
    {
        public ProjectDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(50).WithMessage("Project name cannot exceed 50 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2500).WithMessage("Description cannot exceed 2500 characters.");

            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("You must select a client for this project.");

            RuleFor(x => x.CurrentLead)
                .NotEmpty().WithMessage("You must assign a lead to this project.");
        }
    }
}