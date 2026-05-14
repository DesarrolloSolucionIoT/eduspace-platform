namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;

public class ReportNotFoundException : Exception
{
    public ReportNotFoundException(int id)
        : base($"Report with ID {id} was not found.")
    {
    }
}
