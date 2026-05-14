using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Pipeline.Middleware.Components;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // Maps concrete exception types to HTTP status codes.
    private static readonly Dictionary<Type, int> StatusMap = new()
    {
        // ── IAM ──────────────────────────────────────────────────────────────────
        { typeof(InvalidCredentialsException),        StatusCodes.Status401Unauthorized },
        { typeof(RefreshTokenNotFoundException),      StatusCodes.Status401Unauthorized },
        { typeof(RefreshTokenAlreadyUsedException),   StatusCodes.Status401Unauthorized },
        { typeof(RefreshTokenExpiredException),       StatusCodes.Status401Unauthorized },
        { typeof(AccountNotFoundException),           StatusCodes.Status404NotFound },
        { typeof(InvalidVerificationCodeException),   StatusCodes.Status400BadRequest },

        // ── Profiles ─────────────────────────────────────────────────────────────
        { typeof(InvalidProfileDataException),        StatusCodes.Status400BadRequest },
        { typeof(ProfileNotFoundException),           StatusCodes.Status404NotFound },
        { typeof(TeacherProfileNotFoundException),    StatusCodes.Status404NotFound },
        { typeof(AdminProfileNotFoundException),      StatusCodes.Status404NotFound },

        // ── Spaces & Resource Management ─────────────────────────────────────────
        { typeof(ClassroomNotFoundException),         StatusCodes.Status404NotFound },
        { typeof(ResourceNotFoundException),          StatusCodes.Status404NotFound },
        { typeof(SharedAreaNotFoundException),        StatusCodes.Status404NotFound },
        { typeof(TeacherNotFoundForClassroomException), StatusCodes.Status400BadRequest },
        { typeof(InvalidClassroomDataException),      StatusCodes.Status400BadRequest },
        { typeof(InvalidResourceDataException),       StatusCodes.Status400BadRequest },
        { typeof(InvalidSharedAreaDataException),     StatusCodes.Status400BadRequest },

        // ── Reservation Scheduling ───────────────────────────────────────────────
        { typeof(MeetingNotFoundException),                   StatusCodes.Status404NotFound },
        { typeof(MeetingConflictException),                   StatusCodes.Status409Conflict },
        { typeof(InvalidMeetingScheduleException),            StatusCodes.Status400BadRequest },
        { typeof(TeacherNotFoundForMeetingException),         StatusCodes.Status400BadRequest },
        { typeof(AdministratorNotFoundForMeetingException),   StatusCodes.Status400BadRequest },
        { typeof(ClassroomNotFoundForMeetingException),       StatusCodes.Status400BadRequest },
        { typeof(TeacherAlreadyInMeetingException),           StatusCodes.Status409Conflict },
        { typeof(TeacherNotInMeetingException),               StatusCodes.Status404NotFound },

        // ── Breakdown Management ─────────────────────────────────────────────────
        { typeof(ReportNotFoundException),            StatusCodes.Status404NotFound },
        { typeof(InvalidReportTransitionException),   StatusCodes.Status409Conflict },
        { typeof(InvalidReportDataException),         StatusCodes.Status400BadRequest },
        { typeof(ResourceNotFoundForReportException), StatusCodes.Status400BadRequest },
    };

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var status = ResolveStatus(exception);

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = ResolveTitle(status),
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static int ResolveStatus(Exception exception)
    {
        // Exact type match first
        if (StatusMap.TryGetValue(exception.GetType(), out var code))
            return code;

        // JWT / token exceptions (base + derived)
        if (exception is SecurityTokenExpiredException
            or SecurityTokenInvalidIssuerException
            or SecurityTokenInvalidAudienceException
            or SecurityTokenSignatureKeyNotFoundException
            or SecurityTokenException)
            return StatusCodes.Status401Unauthorized;

        if (exception is UnauthorizedAccessException)
            return StatusCodes.Status401Unauthorized;

        if (exception is ArgumentException)
            return StatusCodes.Status400BadRequest;

        return StatusCodes.Status500InternalServerError;
    }

    private static string ResolveTitle(int status) => status switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status404NotFound => "Not Found",
        StatusCodes.Status409Conflict => "Conflict",
        _ => "An unexpected error occurred"
    };
}
