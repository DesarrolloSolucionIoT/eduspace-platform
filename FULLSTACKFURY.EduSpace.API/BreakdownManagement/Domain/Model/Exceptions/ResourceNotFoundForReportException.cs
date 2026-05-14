namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;

public class ResourceNotFoundForReportException : Exception
{
    public ResourceNotFoundForReportException(int resourceId)
        : base($"Resource with ID {resourceId} does not exist and cannot be used for a report.")
    {
    }
}
