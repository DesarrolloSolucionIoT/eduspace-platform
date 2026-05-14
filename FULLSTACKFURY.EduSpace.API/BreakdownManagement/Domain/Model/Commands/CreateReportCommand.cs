using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;

public record CreateReportCommand
{
    public CreateReportCommand(
        string kindOfReport,
        string description,
        int resourceId,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(kindOfReport))
            throw new InvalidReportDataException("KindOfReport cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidReportDataException("Description cannot be null or empty.");
        if (resourceId <= 0)
            throw new InvalidReportDataException("ResourceId must be greater than 0.");

        KindOfReport = kindOfReport;
        Description = description;
        ResourceId = resourceId;
        CreatedAt = createdAt;
    }

    public string KindOfReport { get; }
    public string Description { get; }
    public int ResourceId { get; }
    public DateTime CreatedAt { get; }
}
