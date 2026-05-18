using System.Reflection;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.ReservationScheduling;

/// <summary>
/// Fluent builder for <see cref="Meeting"/> test instances.
/// All defaults produce a valid future meeting to keep test setup minimal.
/// The only way to produce a "past" meeting for negative tests is via direct
/// construction — this builder always creates future-safe instances.
/// </summary>
public class MeetingBuilder
{
    // Safe future date: far enough from "now" that UTC clock drift won't flip the test.
    private static readonly DateOnly DefaultDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
    private static readonly TimeOnly DefaultStart = new TimeOnly(9, 0);
    private static readonly TimeOnly DefaultEnd = new TimeOnly(10, 0);

    private string _title = "Test Meeting";
    private string _description = "Test Description";
    private DateOnly _date = DefaultDate;
    private TimeOnly _start = DefaultStart;
    private TimeOnly _end = DefaultEnd;
    private int _administratorId = 1;
    private int _classroomId = 1;
    private int? _id;

    public MeetingBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public MeetingBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public MeetingBuilder WithDate(DateOnly date)
    {
        _date = date;
        return this;
    }

    public MeetingBuilder WithSchedule(TimeOnly start, TimeOnly end)
    {
        _start = start;
        _end = end;
        return this;
    }

    public MeetingBuilder WithAdministratorId(int adminId)
    {
        _administratorId = adminId;
        return this;
    }

    public MeetingBuilder WithClassroomId(int classroomId)
    {
        _classroomId = classroomId;
        return this;
    }

    // Simulates post-persistence state. EF assigns Id via auto-increment; in unit tests
    // we set it directly so conflict-detection predicates that compare Ids work correctly.
    public MeetingBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public Meeting Build()
    {
        var meeting = new Meeting(_title, _description, _date, _start, _end, _administratorId, _classroomId);

        if (_id is { } id)
        {
            typeof(Meeting)
                .GetProperty(nameof(Meeting.Id))!
                .SetValue(meeting, id);
        }

        return meeting;
    }
}
