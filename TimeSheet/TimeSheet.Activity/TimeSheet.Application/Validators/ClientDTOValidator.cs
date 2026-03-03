using FluentValidation;
using TimeSheet.Application.DTOs.Client;

namespace TimeSheet.Application.Validators
{
    public class ClientDTOValidator : AbstractValidator<ClientRequestDTO>
    {
        public ClientDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Client name is required.")
                .MaximumLength(50).WithMessage("Client name cannot exceed 50 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(255).WithMessage("City cannot exceed 255 characters.");

            RuleFor(x => x.Zip)
                .NotEmpty().WithMessage("Zip code is required.")
                .MaximumLength(20).WithMessage("Zip code cannot exceed 20 characters.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country must be selected.");
        }
    }
}