using System.ComponentModel.DataAnnotations.Schema;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Entities;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;

// NOTE: Chose option (B) for MeetingDate — its rules (2-hour max, 7am-8pm, no past dates)
// were valid domain invariants. Rather than keeping a separate VO, the logic was inlined
// into EnsureValidSchedule() below so the aggregate enforces them directly on every write.
// DateTime.UtcNow is used (not DateTime.Now) — time comparisons assume UTC-stored dates.
// If the server runs in a non-UTC timezone, callers must convert to UTC before passing dates.
public class Meeting
{
    // ------------------------------------------------------------------
    // Participant management (was in partial MeetingManagement.cs)
    // ------------------------------------------------------------------
    public ICollection<MeetingSession> MeetingParticipants { get; } = new List<MeetingSession>();

    // NotMapped helper kept so existing callers that set TeacherId still compile.
    // TODO: remove in Fase 5 cleanup.
    [NotMapped] public TeacherId TeacherId { get; set; } = new TeacherId(0);

    public void AddTeacher(int teacherId)
    {
        if (MeetingParticipants.Any(mp => mp.TeacherId == teacherId))
            throw new TeacherAlreadyInMeetingException(teacherId, Id);

        MeetingParticipants.Add(new MeetingSession(teacherId, Id));
    }

    public bool RemoveTeacher(int teacherId)
    {
        var participant = MeetingParticipants.FirstOrDefault(mp => mp.TeacherId == teacherId);
        if (participant == null)
            return false;

        MeetingParticipants.Remove(participant);
        return true;
    }

    // Kept for backward compatibility with AddTeacherToMeeting callers.
    public void AddTeacherToMeeting(int teacherId) => AddTeacher(teacherId);

    // Kept for backward compatibility.
    public bool RemoveTeacherFromMeeting(int teacherId) => RemoveTeacher(teacherId);

    public void TeacherIdBuilder(int teacherId)
    {
        TeacherId = new TeacherId(teacherId);
    }

    // ------------------------------------------------------------------
    // Constructors
    // ------------------------------------------------------------------
    public Meeting()
    {
        Title = string.Empty;
        Description = string.Empty;
        AdministratorId = new AdministratorId(0);
        ClassroomId = new ClassroomId(0);
        TeacherId = new TeacherId(0);
    }

    public Meeting(string title, string description, DateOnly date, TimeOnly start, TimeOnly end, int administratorId,
        int classroomId) : this()
    {
        EnsureValidSchedule(date, start, end);
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
        EnsureValidSchedule(command.Date, command.Start, command.End);
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
        EnsureValidSchedule(command.Date, command.Start, command.End);
        Description = command.Description;
        Date = command.Date;
        StartTime = command.Start;
        EndTime = command.End;
        AdministratorId = new AdministratorId(command.AdministratorId);
        ClassroomId = new ClassroomId(command.ClassroomId);
    }

    // ------------------------------------------------------------------
    // Properties
    // ------------------------------------------------------------------
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public AdministratorId AdministratorId { get; private set; }
    public ClassroomId ClassroomId { get; private set; }

    // ------------------------------------------------------------------
    // Update methods
    // ------------------------------------------------------------------
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

    /// <summary>
    /// Atomically updates date and time, enforcing EnsureValidSchedule on every write.
    /// Replaces the old UpdateDate + UpdateTime methods that bypassed invariants.
    /// </summary>
    public void UpdateSchedule(DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        EnsureValidSchedule(date, startTime, endTime);
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
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

    // ------------------------------------------------------------------
    // Domain invariants (replaces MeetingDate VO — chosen option B)
    // Uses DateTime.UtcNow for past-date check. Timezone assumption: UTC.
    // ------------------------------------------------------------------
    private static void EnsureValidSchedule(DateOnly date, TimeOnly start, TimeOnly end)
    {
        if (start >= end)
            throw new InvalidMeetingScheduleException("Start time must be before end time.");

        // Combine to full UTC DateTime for past-date and duration checks
        var startDt = date.ToDateTime(start);
        var endDt = date.ToDateTime(end);

        if (startDt < DateTime.UtcNow)
            throw new InvalidMeetingScheduleException("Meeting cannot be scheduled in the past.");

        if ((endDt - startDt).TotalHours > 2)
            throw new InvalidMeetingScheduleException("A meeting cannot exceed 2 hours.");

        if (start.Hour < 7 || end.Hour > 20 || (end.Hour == 20 && end.Minute > 0))
            throw new InvalidMeetingScheduleException("Meetings must be scheduled between 07:00 and 20:00.");
    }
}
