using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.ReservationScheduling;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.ReservationScheduling.Domain.Model.Aggregates;

public class MeetingTests
{
    // A future date used throughout; far enough ahead that UtcNow drift won't flip tests.
    private static readonly DateOnly FutureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    // ─── Constructor (via builder / full ctor) ───────────────────────────────────

    [Fact]
    public void Constructor_WhenValidData_SetsAllProperties()
    {
        // Arrange
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(10, 0);

        // Act
        var meeting = new Meeting("Math Session", "Algebra review", FutureDate, start, end, 5, 3);

        // Assert
        meeting.Title.Should().Be("Math Session");
        meeting.Description.Should().Be("Algebra review");
        meeting.Date.Should().Be(FutureDate);
        meeting.StartTime.Should().Be(start);
        meeting.EndTime.Should().Be(end);
        meeting.AdministratorId.Should().Be(new AdministratorId(5));
        meeting.ClassroomId.Should().Be(new ClassroomId(3));
    }

    [Fact]
    public void Constructor_WhenStartEqualsEnd_ThrowsInvalidMeetingScheduleException()
    {
        // Arrange
        var time = new TimeOnly(9, 0);

        // Act
        Action act = () => new Meeting("T", "D", FutureDate, time, time, 1, 1);

        // Assert
        act.Should().Throw<InvalidMeetingScheduleException>();
    }

    [Fact]
    public void Constructor_WhenStartAfterEnd_ThrowsInvalidMeetingScheduleException()
    {
        // Arrange
        var start = new TimeOnly(11, 0);
        var end = new TimeOnly(9, 0);

        // Act
        Action act = () => new Meeting("T", "D", FutureDate, start, end, 1, 1);

        // Assert
        act.Should().Throw<InvalidMeetingScheduleException>();
    }

    [Fact]
    public void Constructor_WhenDateIsInThePast_ThrowsInvalidMeetingScheduleException()
    {
        // Arrange — use a clearly past date
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(10, 0);

        // Act
        Action act = () => new Meeting("T", "D", pastDate, start, end, 1, 1);

        // Assert
        act.Should().Throw<InvalidMeetingScheduleException>()
            .WithMessage("*past*");
    }

    [Fact]
    public void Constructor_WhenDurationExceedsTwoHours_ThrowsInvalidMeetingScheduleException()
    {
        // Arrange
        var start = new TimeOnly(8, 0);
        var end = new TimeOnly(10, 1); // 2h 1min

        // Act
        Action act = () => new Meeting("T", "D", FutureDate, start, end, 1, 1);

        // Assert
        act.Should().Throw<InvalidMeetingScheduleException>()
            .WithMessage("*2 hours*");
    }

    [Fact]
    public void Constructor_WhenExactlyTwoHours_DoesNotThrow()
    {
        // Arrange
        var start = new TimeOnly(8, 0);
        var end = new TimeOnly(10, 0); // exactly 2h

        // Act
        Action act = () => new Meeting("T", "D", FutureDate, start, end, 1, 1);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WhenStartBeforeSevenAm_ThrowsInvalidMeetingScheduleException()
    {
        // Arrange
        var start = new TimeOnly(6, 59);
        var end = new TimeOnly(8, 0);

        // Act
        Action act = () => new Meeting("T", "D", FutureDate, start, end, 1, 1);

        // Assert
        act.Should().Throw<InvalidMeetingScheduleException>()
            .WithMessage("*07:00*");
    }

    [Fact]
    public void Constructor_WhenEndAfterEightPm_ThrowsInvalidMeetingScheduleException()
    {
        // Arrange
        var start = new TimeOnly(19, 0);
        var end = new TimeOnly(20, 1);

        // Act
        Action act = () => new Meeting("T", "D", FutureDate, start, end, 1, 1);

        // Assert
        act.Should().Throw<InvalidMeetingScheduleException>()
            .WithMessage("*20:00*");
    }

    [Fact]
    public void Constructor_WhenEndIsExactly20h00_DoesNotThrow()
    {
        // Arrange
        var start = new TimeOnly(19, 0);
        var end = new TimeOnly(20, 0); // 20:00 sharp is allowed

        // Act
        Action act = () => new Meeting("T", "D", FutureDate, start, end, 1, 1);

        // Assert
        act.Should().NotThrow();
    }

    // ─── UpdateTitle ─────────────────────────────────────────────────────────────

    [Fact]
    public void UpdateTitle_WhenNonEmpty_ChangesTitle()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithTitle("Old Title").Build();

        // Act
        meeting.UpdateTitle("New Title");

        // Assert
        meeting.Title.Should().Be("New Title");
    }

    [Fact]
    public void UpdateTitle_WhenNullOrEmpty_DoesNotChangeTitle()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithTitle("Original").Build();

        // Act
        meeting.UpdateTitle(null);

        // Assert
        meeting.Title.Should().Be("Original");
    }

    // ─── UpdateDescription ───────────────────────────────────────────────────────

    [Fact]
    public void UpdateDescription_WhenNonEmpty_ChangesDescription()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithDescription("Old Desc").Build();

        // Act
        meeting.UpdateDescription("New Desc");

        // Assert
        meeting.Description.Should().Be("New Desc");
    }

    [Fact]
    public void UpdateDescription_WhenEmpty_DoesNotChangeDescription()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithDescription("Original").Build();

        // Act
        meeting.UpdateDescription(string.Empty);

        // Assert
        meeting.Description.Should().Be("Original");
    }

    // ─── UpdateSchedule ──────────────────────────────────────────────────────────

    [Fact]
    public void UpdateSchedule_WhenValid_ChangesDateAndTimes()
    {
        // Arrange
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 0), new TimeOnly(10, 0))
            .Build();
        var newDate = FutureDate.AddDays(1);
        var newStart = new TimeOnly(14, 0);
        var newEnd = new TimeOnly(15, 0);

        // Act
        meeting.UpdateSchedule(newDate, newStart, newEnd);

        // Assert
        meeting.Date.Should().Be(newDate);
        meeting.StartTime.Should().Be(newStart);
        meeting.EndTime.Should().Be(newEnd);
    }

    [Fact]
    public void UpdateSchedule_WhenStartAfterEnd_ThrowsInvalidMeetingScheduleException()
    {
        // Arrange
        var meeting = new MeetingBuilder().Build();

        // Act
        Action act = () => meeting.UpdateSchedule(FutureDate, new TimeOnly(11, 0), new TimeOnly(9, 0));

        // Assert
        act.Should().Throw<InvalidMeetingScheduleException>();
    }

    // ─── UpdateAdministrator ─────────────────────────────────────────────────────

    [Fact]
    public void UpdateAdministrator_WhenValidIdAndValidatorReturnsTrue_ChangesAdministratorId()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithAdministratorId(1).Build();

        // Act
        meeting.UpdateAdministrator(99, _ => true);

        // Assert
        meeting.AdministratorId.Should().Be(new AdministratorId(99));
    }

    [Fact]
    public void UpdateAdministrator_WhenValidatorReturnsFalse_DoesNotChangeAdministratorId()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithAdministratorId(1).Build();

        // Act
        meeting.UpdateAdministrator(99, _ => false);

        // Assert
        meeting.AdministratorId.Should().Be(new AdministratorId(1));
    }

    [Fact]
    public void UpdateAdministrator_WhenNullId_DoesNotChangeAdministratorId()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithAdministratorId(1).Build();

        // Act
        meeting.UpdateAdministrator(null, _ => true);

        // Assert
        meeting.AdministratorId.Should().Be(new AdministratorId(1));
    }

    // ─── UpdateClassroom ─────────────────────────────────────────────────────────

    [Fact]
    public void UpdateClassroom_WhenValidIdAndValidatorReturnsTrue_ChangesClassroomId()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithClassroomId(1).Build();

        // Act
        meeting.UpdateClassroom(42, _ => true);

        // Assert
        meeting.ClassroomId.Should().Be(new ClassroomId(42));
    }

    [Fact]
    public void UpdateClassroom_WhenValidatorReturnsFalse_DoesNotChangeClassroomId()
    {
        // Arrange
        var meeting = new MeetingBuilder().WithClassroomId(1).Build();

        // Act
        meeting.UpdateClassroom(42, _ => false);

        // Assert
        meeting.ClassroomId.Should().Be(new ClassroomId(1));
    }

    // ─── AddTeacher ──────────────────────────────────────────────────────────────

    [Fact]
    public void AddTeacher_WhenTeacherNotPresent_AddsToParticipants()
    {
        // Arrange
        var meeting = new MeetingBuilder().Build();

        // Act
        meeting.AddTeacher(7);

        // Assert
        meeting.MeetingParticipants.Should().ContainSingle(mp => mp.TeacherId == 7);
    }

    [Fact]
    public void AddTeacher_WhenTeacherAlreadyPresent_ThrowsTeacherAlreadyInMeetingException()
    {
        // Arrange
        var meeting = new MeetingBuilder().Build();
        meeting.AddTeacher(7);

        // Act
        Action act = () => meeting.AddTeacher(7);

        // Assert
        act.Should().Throw<TeacherAlreadyInMeetingException>();
    }

    [Fact]
    public void AddTeacher_WhenMultipleTeachersAdded_AllAppearInParticipants()
    {
        // Arrange
        var meeting = new MeetingBuilder().Build();

        // Act
        meeting.AddTeacher(1);
        meeting.AddTeacher(2);
        meeting.AddTeacher(3);

        // Assert
        meeting.MeetingParticipants.Should().HaveCount(3);
    }

    // ─── RemoveTeacher ───────────────────────────────────────────────────────────

    [Fact]
    public void RemoveTeacher_WhenTeacherPresent_ReturnsTrueAndRemovesFromParticipants()
    {
        // Arrange
        var meeting = new MeetingBuilder().Build();
        meeting.AddTeacher(7);

        // Act
        var result = meeting.RemoveTeacher(7);

        // Assert
        result.Should().BeTrue();
        meeting.MeetingParticipants.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTeacher_WhenTeacherNotPresent_ReturnsFalse()
    {
        // Arrange
        var meeting = new MeetingBuilder().Build();

        // Act
        var result = meeting.RemoveTeacher(999);

        // Assert
        result.Should().BeFalse();
    }
}
