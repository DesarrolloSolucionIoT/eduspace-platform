namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;

public class InvalidReportDataException : Exception
{
    public InvalidReportDataException(string message)
        : base(message)
    {
    }
}
