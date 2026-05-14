namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;

public record ResourceId
{
    public ResourceId(int id)
    {
        if (id <= 0) throw new ArgumentException("Resource ID must be greater than 0.");
        Id = id;
    }

    public int Id { get; init; }
}
