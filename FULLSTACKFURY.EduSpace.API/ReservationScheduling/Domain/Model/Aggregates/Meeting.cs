using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;

public partial class Meeting
{
    public Meeting()
    {
        Title = string.Empty;
        Description = string.Empty;
        AdministratorId = default!;
        ClassroomId = default!;
        TeacherId = default!;
    }

    public Meeting(string title, string description, DateOnly date, TimeOnly start, TimeOnly end, int administratorId,
        int classroomId) : this()
    {
        Title = title;
        Description = description;
        Date = date;
        StartTime = start;
        EndTime = end;
        AdministratorId = new AdministratorId(administratorId);
        ClassroomId = new ClassroomId(classroomId);
    }

    public Meeting(CreateMeetingCommand command) : this()
    {
        Title = command.Title;
        Description = command.Description;
        Date = command.Date;
        StartTime = command.Start;
        EndTime = command.End;
        AdministratorId = new AdministratorId(command.AdministratorId);
        ClassroomId = new ClassroomId(command.ClassroomId);
    }

    public Meeting(UpdateMeetingCommand command) : this()
    {
        Description = command.Description;
        Date = command.Date;
        StartTime = command.Start;
        EndTime = command.End;
        AdministratorId = new AdministratorId(command.AdministratorId);
        ClassroomId = new ClassroomId(command.ClassroomId);
    }

    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateOnly Date { get; private set; }

    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    // public List<Teacher> Teachers { get; private set; } 
    public AdministratorId AdministratorId { get; private set; }
    public ClassroomId ClassroomId { get; private set; }

    public void UpdateTitle(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            Title = title;
    }

    public void UpdateDescription(string? description)
    {
        if (!string.IsNullOrEmpty(description))
            Description = description;
    }

    public void UpdateDate(DateOnly? date)
    {
        if (date.HasValue)
            Date = date.Value;
    }

    public void UpdateTime(TimeOnly? start, TimeOnly? end)
    {
        if (start.HasValue) StartTime = start.Value;
        if (end.HasValue) EndTime = end.Value;
    }

    public void UpdateAdministrator(int? adminId, Func<int, bool> validateAdmin)
    {
        if (adminId.HasValue && validateAdmin(adminId.Value))
            AdministratorId = new AdministratorId(adminId.Value);
    }

    public void UpdateClassroom(int? classroomId, Func<int, bool> validateClassroom)
    {
        if (classroomId.HasValue && validateClassroom(classroomId.Value))
            ClassroomId = new ClassroomId(classroomId.Value);
    }
}