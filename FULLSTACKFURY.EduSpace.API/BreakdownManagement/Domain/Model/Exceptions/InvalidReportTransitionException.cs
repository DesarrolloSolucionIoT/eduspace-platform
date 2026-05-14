namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;

public class InvalidReportTransitionException : Exception
{
    public InvalidReportTransitionException(string fromStatus, string toStatus)
        : base($"Cannot transition report from '{fromStatus}' to '{toStatus}'.")
    {
    }
}
