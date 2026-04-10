namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;

public record ReportStatus
{
    public static readonly ReportStatus EnProceso = new("in progress");
    public static readonly ReportStatus Completado = new("completed");

    private ReportStatus(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ReportStatus FromString(string statusStr)
    {
        return statusStr switch
        {
            "in progress" => EnProceso,
            "completed" => Completado,
            _ => throw new ArgumentException($"{statusStr} is not a valid report status.")
        };
    }
}