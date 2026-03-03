using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeSheet.WebAPI.Middleware;

public record ExceptionDetails(int StatusCode, string Title);

public static class ExceptionDictionary
{
    private static readonly Dictionary<Type, ExceptionDetails> Mappings = new()
    {
        { typeof(NotFoundException), new(StatusCodes.Status404NotFound, "Resource Not Found") },
        { typeof(BadRequestException), new(StatusCodes.Status400BadRequest, "Invalid Request") },
        { typeof(ConflictException), new(StatusCodes.Status409Conflict, "Resource Already Exists") },
        { typeof(ValidationException), new(StatusCodes.Status400BadRequest, "Validation Error") },
        { typeof(ForbiddenException), new(StatusCodes.Status403Forbidden, "Forbidden Access") },
        { typeof(DbUpdateException), new(StatusCodes.Status422UnprocessableEntity, "Database Constraint Violation") },
        { typeof(TaskCanceledException), new(StatusCodes.Status408RequestTimeout, "Request Timeout") },
        { typeof(OperationCanceledException), new(StatusCodes.Status408RequestTimeout, "Request Timeout") },
        { typeof(UnauthorizedAccessException), new(StatusCodes.Status401Unauthorized, "Access Denied") }
    };

    public static ExceptionDetails GetDetails(Exception exception)
    {
        return Mappings.GetValueOrDefault(
            exception.GetType(),
            new ExceptionDetails(StatusCodes.Status500InternalServerError, "Server Error")
        );
    }
}