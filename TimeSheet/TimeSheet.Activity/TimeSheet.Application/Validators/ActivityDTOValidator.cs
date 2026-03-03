using FluentValidation;
using System;
using System.Linq;
using TimeSheet.Application.DTOs.Activity;

namespace TimeSheet.Application.Validators
{
    public class ActivityDTOValidator : AbstractValidator<ActivityRequestDTO>
    {
        public ActivityDTOValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2500).WithMessage("Description cannot exceed 2500 characters.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Date cannot be too far in the future.");

            RuleFor(x => x.Time)
                .NotEmpty().WithMessage("Time is required.")
                .InclusiveBetween(0.5m, 24.0m).WithMessage("Time must be between 0.5 and 24 hours.");

            RuleFor(x => x.OverTime)
                .NotNull().WithMessage("Overtime field is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Overtime cannot be negative.");

            RuleFor(x => x)
                .Must(x => x.Time + x.OverTime <= 24m)
                .WithMessage("Total hours in a day cannot exceed 24.");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project must be selected.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category must be selected.");
        }
    }
}