using FluentAssertions;
using NSubstitute;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.ReservationScheduling;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.ReservationScheduling.Application.Internal.QueryServices;

public class MeetingQueryServiceTests
{
    private static readonly DateOnly FutureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    private (MeetingQueryService sut, IMeetingRepository repo) CreateSut()
    {
        var repo = Substitute.For<IMeetingRepository>();
        var sut = new MeetingQueryService(repo);
        return (sut, repo);
    }

    private Meeting BuildMeeting() => new MeetingBuilder().Build();

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(GetAllMeetingsQuery)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleGetAll_WhenMeetingsExist_ReturnsPopulatedList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        var meetings = new List<Meeting> { BuildMeeting(), BuildMeeting() };
        repo.ListAsync().Returns(Task.FromResult<IEnumerable<Meeting>>(meetings));

        // Act
        var result = await sut.Handle(new GetAllMeetingsQuery());

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleGetAll_WhenNoMeetingsExist_ReturnsEmptyList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        repo.ListAsync().Returns(Task.FromResult<IEnumerable<Meeting>>(new List<Meeting>()));

        // Act
        var result = await sut.Handle(new GetAllMeetingsQuery());

        // Assert
        result.Should().BeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(GetMeetingByIdQuery)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleGetById_WhenMeetingExists_ReturnsSingleElementList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        var meeting = BuildMeeting();
        repo.FindByIdAsync(1).Returns(Task.FromResult<Meeting?>(meeting));

        // Act
        var result = await sut.Handle(new GetMeetingByIdQuery(1));

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task HandleGetById_WhenMeetingNotFound_ReturnsEmptyList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        repo.FindByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Meeting?>(null));

        // Act
        var result = await sut.Handle(new GetMeetingByIdQuery(999));

        // Assert
        result.Should().BeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(GetAllMeetingByAdminIdQuery)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleGetByAdminId_WhenMeetingsExist_ReturnsPopulatedList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        var meetings = new List<Meeting> { BuildMeeting(), BuildMeeting(), BuildMeeting() };
        repo.FindAllByAdminIdAsync(5).Returns(Task.FromResult<IEnumerable<Meeting>>(meetings));

        // Act
        var result = await sut.Handle(new GetAllMeetingByAdminIdQuery(5));

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task HandleGetByAdminId_WhenNoMeetingsForAdmin_ReturnsEmptyList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        repo.FindAllByAdminIdAsync(Arg.Any<int>()).Returns(Task.FromResult<IEnumerable<Meeting>>(new List<Meeting>()));

        // Act
        var result = await sut.Handle(new GetAllMeetingByAdminIdQuery(999));

        // Assert
        result.Should().BeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Handle(GetAllMeetingByTeacherIdQuery)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task HandleGetByTeacherId_WhenMeetingsExist_ReturnsPopulatedList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        var meetings = new List<Meeting> { BuildMeeting() };
        repo.FindAllByTeacherIdAsync(7).Returns(Task.FromResult<IEnumerable<Meeting>>(meetings));

        // Act
        var result = await sut.Handle(new GetAllMeetingByTeacherIdQuery(7));

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task HandleGetByTeacherId_WhenNoMeetingsForTeacher_ReturnsEmptyList()
    {
        // Arrange
        var (sut, repo) = CreateSut();
        repo.FindAllByTeacherIdAsync(Arg.Any<int>()).Returns(Task.FromResult<IEnumerable<Meeting>>(new List<Meeting>()));

        // Act
        var result = await sut.Handle(new GetAllMeetingByTeacherIdQuery(999));

        // Assert
        result.Should().BeEmpty();
    }
}
