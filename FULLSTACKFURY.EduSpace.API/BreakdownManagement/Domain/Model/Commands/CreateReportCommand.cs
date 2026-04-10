namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;

public record CreateReportCommand
{
    public CreateReportCommand(
        string kindOfReport,
        string description,
        int resourceId,
        DateTime createdAt)
    {
        KindOfReport = kindOfReport ?? throw new ArgumentException("El tipo de informe no puede ser nulo o vacío.");
        Description = description;
        ResourceId = resourceId > 0 ? resourceId : throw new ArgumentException("ResourceId debe ser mayor que 0.");
        CreatedAt = createdAt;
    }

    public string KindOfReport { get; }
    public string Description { get; }
    public int ResourceId { get; }
    public DateTime CreatedAt { get; }
}