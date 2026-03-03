using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using TimeSheetValidationException = TimeSheet.Application.Common.Exceptions.ValidationException;

namespace TimeSheet.Application.Services
{
    public abstract class BaseService
    {
        protected async Task ValidateAsync<T>(IValidator<T> validator, T dto)
        {
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
                throw new TimeSheetValidationException(errorMessages);
            }
        }
    }
}

