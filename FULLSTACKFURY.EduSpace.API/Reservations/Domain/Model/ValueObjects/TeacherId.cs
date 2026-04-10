namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.ValueObjects;

public record TeacherId
{
    public TeacherId(int TeacherIdentifier)
    {
        if (TeacherIdentifier <= 0) throw new ArgumentException("Teacher Id cannot be less than or equal to 0");
        this.TeacherIdentifier = TeacherIdentifier;
    }

    private TeacherId()
    {
    }

    public int TeacherIdentifier { get; init; }
}