using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeSheet.Application.Exceptions;

namespace TimeSheet.Infrastructure.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                BadRequestException => (StatusCodes.Status400BadRequest, "Invalid Request"),
                ConflictException => (StatusCodes.Status409Conflict, "Resource Already Exists"),
                ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden Access"),
                DbUpdateException => (StatusCodes.Status409Conflict, "Database Constraint Violation"),
                TaskCanceledException or OperationCanceledException => (StatusCodes.Status408RequestTimeout, "Request Timeout"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Access Denied"),
                
                _ => (StatusCodes.Status500InternalServerError, "Server Error")
            };

            _logger.LogError(
                exception, "Exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error",
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
