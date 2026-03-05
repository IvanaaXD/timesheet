using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using TimeSheetValidationException = TimeSheet.Application.Common.Exceptions.ValidationException;

namespace TimeSheet.Application.Common.Extensions
{
    public static class ValidationExtensions
    {
        public static async Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T dto)
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