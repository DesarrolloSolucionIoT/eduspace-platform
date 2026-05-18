using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.ReservationScheduling;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.ReservationScheduling.Application.Internal.CommandServices;

public class MeetingCommandServiceTests
{
    // ─── Shared fixtures ────────────────────────────────────────────────────────

    private static readonly DateOnly FutureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
    private static readonly TimeOnly NineAm = new TimeOnly(9, 0);
    private static readonly TimeOnly TenAm = new TimeOnly(10, 0);

    // Helper: returns a CreateMeetingCommand with all-valid defaults.
    private static CreateMeetingCommand ValidCreateCommand(
        DateOnly? date = null,
        TimeOnly? start = null,
        TimeOnly? end = null,
        int adminId = 1,
        int classroomId = 1) =>
        new CreateMeetingCommand(
            "Test Meeting",
            "Test Description",
            date ?? FutureDate,
            start ?? NineAm,
            end ?? TenAm,
            adminId,
            classroomId);

    // Helper: returns an UpdateMeetingCommand with all-valid defaults.
    private static UpdateMeetingCommand ValidUpdateCommand(
        int meetingId = 1,
        DateOnly? date = null,
        TimeOnly? start = null,
        TimeOnly? end = null,
        int adminId = 1,
        int classroomId = 1) =>
        new UpdateMeetingCommand(
            meetingId,
            "Updated Title",
            "Updated Description",
            date ?? FutureDate,
            start ?? NineAm,
            end ?? TenAm,
            adminId,
            classroomId);

    private (MeetingCommandService sut,
             IMeetingRepository repo,
             IUnitOfWork uow,
             IExternalProfileService profileSvc,
             IExternalClassroomService classroomSvc) CreateSut()
    {
        var repo = Substitute.For<IMeetingRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        var profileSvc = Substitute.For<IExternalProfileService>();
        var classroomSvc = Substitute.For<IExternalClassroomService>();

        var sut = new MeetingCommandService(repo, uow, profileSvc, classroomSvc);
        return (sut, repo, uow, profileSvc, classroomSvc);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(CreateMeetingCommand)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleCreate_WhenAdminAndClassroomExist_ReturnsMeeting()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, classroomSvc) = CreateSut();
        profileSvc.ValidateAdminIdExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        classroomSvc.ValidateClassroomIdAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var command = ValidCreateCommand();

        // Act
        var result = await sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Meeting");
    }

    [Fact]
    public async Task HandleCreate_WhenAdminAndClassroomExist_CallsAddAndComplete()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, classroomSvc) = CreateSut();
        profileSvc.ValidateAdminIdExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        classroomSvc.ValidateClassroomIdAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await sut.Handle(ValidCreateCommand());

        // Assert
        await repo.Received(1).AddAsync(Arg.Any<Meeting>());
        await uow.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task HandleCreate_WhenAdminDoesNotExist_ThrowsAdministratorNotFoundForMeetingException()
    {
        // Arrange
        var (sut, _, _, profileSvc, classroomSvc) = CreateSut();
        profileSvc.ValidateAdminIdExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(false));
        classroomSvc.ValidateClassroomIdAsync(Arg.Any<int>()).Returns(Task.FromResult(true));

        // Act
        Func<Task> act = () => sut.Handle(ValidCreateCommand());

        // Assert
        await act.Should().ThrowAsync<AdministratorNotFoundForMeetingException>();
    }

    [Fact]
    public async Task HandleCreate_WhenClassroomDoesNotExist_ThrowsClassroomNotFoundForMeetingException()
    {
        // Arrange
        var (sut, _, _, profileSvc, classroomSvc) = CreateSut();
        profileSvc.ValidateAdminIdExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        classroomSvc.ValidateClassroomIdAsync(Arg.Any<int>()).Returns(Task.FromResult(false));

        // Act
        Func<Task> act = () => sut.Handle(ValidCreateCommand());

        // Assert
        await act.Should().ThrowAsync<ClassroomNotFoundForMeetingException>();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(DeleteMeetingCommand)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleDelete_WhenMeetingExists_RemovesAndCompletes()
    {
        // Arrange
        var (sut, repo, uow, _, _) = CreateSut();
        var meeting = new MeetingBuilder().Build();
        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await sut.Handle(new DeleteMeetingCommand(1));

        // Assert
        repo.Received(1).Remove(meeting);
        await uow.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task HandleDelete_WhenMeetingNotFound_ThrowsMeetingNotFoundException()
    {
        // Arrange
        var (sut, repo, _, _, _) = CreateSut();
        repo.FindByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Meeting?>(null));

        // Act
        Func<Task> act = () => sut.Handle(new DeleteMeetingCommand(999));

        // Assert
        await act.Should().ThrowAsync<MeetingNotFoundException>();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(UpdateMeetingCommand)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleUpdate_WhenMeetingExists_UpdatesTitleAndCompletes()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, classroomSvc) = CreateSut();
        var existing = new MeetingBuilder().Build();
        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(existing));
        profileSvc.ValidateAdminIdExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        classroomSvc.ValidateClassroomIdAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var command = ValidUpdateCommand(meetingId: 1);

        // Act
        var result = await sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        await uow.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task HandleUpdate_WhenMeetingNotFound_ThrowsMeetingNotFoundException()
    {
        // Arrange
        var (sut, repo, _, _, _) = CreateSut();
        repo.FindByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Meeting?>(null));

        // Act
        Func<Task> act = () => sut.Handle(ValidUpdateCommand(meetingId: 999));

        // Assert
        await act.Should().ThrowAsync<MeetingNotFoundException>();
    }

    [Fact]
    public async Task HandleUpdate_WhenScheduleChangedAndParticipantHasConflict_ThrowsMeetingConflictException()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, classroomSvc) = CreateSut();

        // The meeting being updated — Id 1 simulates post-persistence state.
        var existing = new MeetingBuilder()
            .WithId(1)
            .WithDate(FutureDate)
            .WithSchedule(NineAm, TenAm)
            .Build();
        existing.AddTeacher(7);

        // Conflicting meeting for the same teacher — distinct Id, overlapping window.
        var conflicting = new MeetingBuilder()
            .WithId(2)
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 30), new TimeOnly(10, 30))
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(existing));
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting> { conflicting }));
        profileSvc.ValidateAdminIdExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        classroomSvc.ValidateClassroomIdAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Command changes the schedule into a window that overlaps `conflicting` (9:30–10:30).
        var command = ValidUpdateCommand(
            meetingId: 1,
            date: FutureDate,
            start: new TimeOnly(9, 30),
            end: new TimeOnly(10, 30));

        // Act
        Func<Task> act = () => sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<MeetingConflictException>();
    }

    [Fact]
    public async Task HandleUpdate_WhenScheduleUnchanged_SkipsConflictCheck()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, classroomSvc) = CreateSut();

        var existing = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(NineAm, TenAm)
            .Build();
        existing.AddTeacher(7);

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(existing));
        profileSvc.ValidateAdminIdExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        classroomSvc.ValidateClassroomIdAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Same date and same start/end as the existing meeting → no change → no conflict query
        var command = ValidUpdateCommand(meetingId: 1, date: FutureDate, start: NineAm, end: TenAm);

        // Act
        await sut.Handle(command);

        // Assert — FindAllByTeacherIdAsync should NOT have been called
        await repo.DidNotReceive().FindAllByTeacherIdAsync(Arg.Any<int>());
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(AddTeacherToMeetingCommand) — conflict-detection scenarios
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleAddTeacher_WhenNoConflict_AddsTeacherAndCompletes()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(NineAm, TenAm)
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        // No conflicting meetings for teacher 7
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting>()));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await uow.Received(1).CompleteAsync();
        meeting.MeetingParticipants.Should().ContainSingle(mp => mp.TeacherId == 7);
    }

    [Fact]
    public async Task HandleAddTeacher_WhenMeetingNotFound_ThrowsMeetingNotFoundException()
    {
        // Arrange
        var (sut, repo, _, _, _) = CreateSut();
        repo.FindByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Meeting?>(null));

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 999));

        // Assert
        await act.Should().ThrowAsync<MeetingNotFoundException>();
    }

    [Fact]
    public async Task HandleAddTeacher_WhenTeacherNotFound_ThrowsTeacherNotFoundForMeetingException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder().Build();
        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(false));

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<TeacherNotFoundForMeetingException>();
    }

    [Fact]
    public async Task HandleAddTeacher_WhenTeacherAlreadyInMeeting_ThrowsTeacherAlreadyInMeetingException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder().Build();
        meeting.AddTeacher(7); // already added in-memory

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<TeacherAlreadyInMeetingException>();
    }

    // ─── Overlap scenarios (6 timestamp cases) ──────────────────────────────────

    // Scenario A: Existing meeting fully contains the new one.
    // existing: 09:00–11:00 | new: 09:30–10:30 → CONFLICT
    [Fact]
    public async Task HandleAddTeacher_WhenExistingContainsNewMeeting_ThrowsMeetingConflictException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 30), new TimeOnly(10, 30))
            .Build();
        var existing = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 0), new TimeOnly(11, 0))
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting> { existing }));

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<MeetingConflictException>();
    }

    // Scenario B: New meeting starts before existing ends (partial overlap — start inside).
    // existing: 09:00–10:00 | new: 09:30–10:30 → CONFLICT
    [Fact]
    public async Task HandleAddTeacher_WhenNewStartsInsideExisting_ThrowsMeetingConflictException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 30), new TimeOnly(10, 30))
            .Build();
        var existing = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 0), new TimeOnly(10, 0))
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting> { existing }));

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<MeetingConflictException>();
    }

    // Scenario C: New meeting ends after existing starts (partial overlap — end inside).
    // existing: 10:00–11:00 | new: 09:30–10:30 → CONFLICT
    [Fact]
    public async Task HandleAddTeacher_WhenNewEndsInsideExisting_ThrowsMeetingConflictException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 30), new TimeOnly(10, 30))
            .Build();
        var existing = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(10, 0), new TimeOnly(11, 0))
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting> { existing }));

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<MeetingConflictException>();
    }

    // Scenario D: New meeting fully contains the existing one.
    // existing: 09:30–10:00 | new: 09:00–10:30 → CONFLICT
    [Fact]
    public async Task HandleAddTeacher_WhenNewContainsExistingMeeting_ThrowsMeetingConflictException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 0), new TimeOnly(10, 30))
            .Build();
        var existing = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 30), new TimeOnly(10, 0))
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting> { existing }));

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<MeetingConflictException>();
    }

    // Scenario E: Back-to-back — new ends exactly when existing starts → NO conflict.
    // existing: 10:00–11:00 | new: 09:00–10:00 → NO CONFLICT (boundary is exclusive)
    [Fact]
    public async Task HandleAddTeacher_WhenNewEndsExactlyWhenExistingStarts_DoesNotConflict()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(9, 0), new TimeOnly(10, 0))
            .Build();
        var existing = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(new TimeOnly(10, 0), new TimeOnly(11, 0))
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting> { existing }));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert — no exception expected
        await act.Should().NotThrowAsync();
    }

    // Scenario F: Different date — exact same time window → NO conflict.
    [Fact]
    public async Task HandleAddTeacher_WhenSameTimeButDifferentDate_DoesNotConflict()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder()
            .WithDate(FutureDate)
            .WithSchedule(NineAm, TenAm)
            .Build();
        var existing = new MeetingBuilder()
            .WithDate(FutureDate.AddDays(1)) // next day
            .WithSchedule(NineAm, TenAm)
            .Build();

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(
            new List<Meeting> { existing }));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        Func<Task> act = () => sut.Handle(new AddTeacherToMeetingCommand(7, 1));

        // Assert
        await act.Should().NotThrowAsync();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(RemoveTeacherFromMeetingCommand)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleRemoveTeacher_WhenTeacherIsParticipant_RemovesAndCompletes()
    {
        // Arrange
        var (sut, repo, uow, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder().Build();
        meeting.AddTeacher(7);

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await sut.Handle(new RemoveTeacherFromMeetingCommand(7, 1));

        // Assert
        meeting.MeetingParticipants.Should().BeEmpty();
        await uow.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task HandleRemoveTeacher_WhenMeetingNotFound_ThrowsMeetingNotFoundException()
    {
        // Arrange
        var (sut, repo, _, _, _) = CreateSut();
        repo.FindByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Meeting?>(null));

        // Act
        Func<Task> act = () => sut.Handle(new RemoveTeacherFromMeetingCommand(7, 999));

        // Assert
        await act.Should().ThrowAsync<MeetingNotFoundException>();
    }

    [Fact]
    public async Task HandleRemoveTeacher_WhenTeacherDoesNotExist_ThrowsTeacherNotFoundForMeetingException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder().Build();
        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(Arg.Any<int>()).Returns(Task.FromResult(false));

        // Act
        Func<Task> act = () => sut.Handle(new RemoveTeacherFromMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<TeacherNotFoundForMeetingException>();
    }

    [Fact]
    public async Task HandleRemoveTeacher_WhenTeacherNotInMeeting_ThrowsTeacherNotInMeetingException()
    {
        // Arrange
        var (sut, repo, _, profileSvc, _) = CreateSut();
        var meeting = new MeetingBuilder().Build(); // no participants

        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));
        profileSvc.ValidateTeacherExistenceAsync(7).Returns(Task.FromResult(true));

        // Act
        Func<Task> act = () => sut.Handle(new RemoveTeacherFromMeetingCommand(7, 1));

        // Assert
        await act.Should().ThrowAsync<TeacherNotInMeetingException>();
    }

}
