using FluentValidation;
using TimeSheet.Application.DTOs.Member; 

namespace TimeSheet.Application.Validators
{
    public class MemberDTOValidator : AbstractValidator<MemberRequestDTO>
    {
        public MemberDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.HoursPerWeek)
                .NotNull().WithMessage("Hours per week is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Hours per week cannot be negative.")
                .LessThanOrEqualTo(168).WithMessage("Hours per week cannot exceed 168."); // Max sati u nedelji

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status selected.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role selected.");
        }
    }
}