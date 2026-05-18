using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.BreakdownManagement;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.BreakdownManagement.Application.Internal.QueryServices;

public class ReportQueryServiceTests
{
    private readonly IReportRepository _reportRepository;
    private readonly ReportQueryService _sut;

    public ReportQueryServiceTests()
    {
        _reportRepository = Substitute.For<IReportRepository>();
        _sut = new ReportQueryService(_reportRepository);
    }

    // ── Handle(GetReportByIdQuery) ────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetReportByIdQuery_ReportExists_ReturnsReport()
    {
        // Arrange
        var report = new ReportBuilder().WithResourceId(1).Build();
        var query = new GetReportByIdQuery(1);
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(report));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(report);
    }

    [Fact]
    public async Task Handle_GetReportByIdQuery_ReportDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetReportByIdQuery(999);
        _reportRepository.FindByIdAsync(999).Returns(Task.FromResult<Report?>(null));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetReportByIdQuery_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var query = new GetReportByIdQuery(7);
        _reportRepository.FindByIdAsync(7).Returns(Task.FromResult<Report?>(null));

        // Act
        await _sut.Handle(query);

        // Assert
        await _reportRepository.Received(1).FindByIdAsync(7);
    }

    // ── Handle(GetAllReportsQuery) ────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetAllReportsQuery_NoReports_ReturnsEmptyEnumerable()
    {
        // Arrange
        var query = new GetAllReportsQuery();
        _reportRepository.ListAsync().Returns(Task.FromResult<IEnumerable<Report>>([]));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAllReportsQuery_MultipleReports_ReturnsAllReports()
    {
        // Arrange
        var reports = new List<Report>
        {
            new ReportBuilder().WithKindOfReport("Electrical").WithResourceId(1).Build(),
            new ReportBuilder().WithKindOfReport("Water leak").WithResourceId(2).Build(),
            new ReportBuilder().WithKindOfReport("Fire alarm").WithResourceId(3).Build()
        };
        var query = new GetAllReportsQuery();
        _reportRepository.ListAsync().Returns(Task.FromResult<IEnumerable<Report>>(reports));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_GetAllReportsQuery_CallsListAsync()
    {
        // Arrange
        var query = new GetAllReportsQuery();
        _reportRepository.ListAsync().Returns(Task.FromResult<IEnumerable<Report>>([]));

        // Act
        await _sut.Handle(query);

        // Assert
        await _reportRepository.Received(1).ListAsync();
    }

    // ── Handle(GetAllReportsByResourceIdQuery) ────────────────────────────────

    [Fact]
    public async Task Handle_GetAllReportsByResourceIdQuery_NoReportsForResource_ReturnsEmptyEnumerable()
    {
        // Arrange
        var query = new GetAllReportsByResourceIdQuery(5);
        _reportRepository.FindAllByResourceIdAsync(5).Returns(Task.FromResult<IEnumerable<Report>>([]));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAllReportsByResourceIdQuery_ReportsExistForResource_ReturnsMatchingReports()
    {
        // Arrange
        var reports = new List<Report>
        {
            new ReportBuilder().WithResourceId(5).WithKindOfReport("Broken door").Build(),
            new ReportBuilder().WithResourceId(5).WithKindOfReport("Broken window").Build()
        };
        var query = new GetAllReportsByResourceIdQuery(5);
        _reportRepository.FindAllByResourceIdAsync(5).Returns(Task.FromResult<IEnumerable<Report>>(reports));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAllReportsByResourceIdQuery_CallsRepositoryWithCorrectResourceId()
    {
        // Arrange
        var query = new GetAllReportsByResourceIdQuery(8);
        _reportRepository.FindAllByResourceIdAsync(8).Returns(Task.FromResult<IEnumerable<Report>>([]));

        // Act
        await _sut.Handle(query);

        // Assert
        await _reportRepository.Received(1).FindAllByResourceIdAsync(8);
    }

    [Fact]
    public async Task Handle_GetAllReportsByResourceIdQuery_ReturnsOnlyReportsForThatResource()
    {
        // Arrange
        var targetResourceReports = new List<Report>
        {
            new ReportBuilder().WithResourceId(3).Build()
        };
        var query = new GetAllReportsByResourceIdQuery(3);
        _reportRepository.FindAllByResourceIdAsync(3)
            .Returns(Task.FromResult<IEnumerable<Report>>(targetResourceReports));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().AllSatisfy(r => r.ResourceId.Id.Should().Be(3));
    }
}
