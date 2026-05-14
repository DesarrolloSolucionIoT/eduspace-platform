using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.CommandServices;

public class ReportCommandService : IReportCommandService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExternalResourceService _externalResourceService;
    private readonly ILogger<ReportCommandService> _logger;

    public ReportCommandService(
        IReportRepository reportRepository,
        IUnitOfWork unitOfWork,
        IExternalResourceService externalResourceService,
        ILogger<ReportCommandService> logger)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
        _externalResourceService = externalResourceService;
        _logger = logger;
    }

    public async Task<Report?> Handle(CreateReportCommand command)
    {
        var resourceExists = await _externalResourceService.ValidateResourceExistsAsync(command.ResourceId);
        if (!resourceExists)
            throw new ResourceNotFoundForReportException(command.ResourceId);

        var report = new Report(command);
        await _reportRepository.AddAsync(report);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Report {ReportId} created for resource {ResourceId}.", report.Id, command.ResourceId);
        return report;
    }

    public async Task<Report?> Handle(UpdateReportCommand command)
    {
        var report = await _reportRepository.FindByIdAsync(command.Id);
        if (report is null)
            throw new ReportNotFoundException(command.Id);

        // Update editable fields only.
        report.Update(command);

        // Apply status transition when a target status is specified.
        if (!string.IsNullOrWhiteSpace(command.TargetStatus))
        {
            var targetStatus = ReportStatus.FromString(command.TargetStatus);
            if (targetStatus == ReportStatus.EnProceso)
                report.MarkAsInProgress();
            else if (targetStatus == ReportStatus.Completado)
                report.MarkAsCompleted();
            else
                throw new InvalidReportTransitionException(report.Status.Value, command.TargetStatus);
        }

        _reportRepository.Update(report);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Report {ReportId} updated.", report.Id);
        return report;
    }

    public async Task Handle(DeleteReportCommand command)
    {
        var report = await _reportRepository.FindByIdAsync(command.Id);
        if (report is null)
            throw new ReportNotFoundException(command.Id);

        _reportRepository.Remove(report);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Report {ReportId} deleted.", command.Id);
    }
}
