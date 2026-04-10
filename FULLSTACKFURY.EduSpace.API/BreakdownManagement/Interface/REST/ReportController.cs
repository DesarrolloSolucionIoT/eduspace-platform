using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Resources;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ReportsController(IReportCommandService reportCommandService, IReportQueryService reportQueryService)
    : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a report",
        Description = "Creates a report for a specific resource",
        OperationId = "CreateReport"
    )]
    [SwaggerResponse(201, "The report was created", typeof(ReportResource))]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportResource resource)
    {
        var createReportCommand = CreateReportCommandFromResourceAssembler.ToCommandFromResource(resource);
        var report = await reportCommandService.Handle(createReportCommand);

        if (report is null) return BadRequest();

        var reportResource = ReportResourceFromEntityAssembler.ToResourceFromEntity(report);
        return Ok(reportResource);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReports()
    {
        var getAllReportsQuery = new GetAllReportsQuery();
        var reports = await reportQueryService.Handle(getAllReportsQuery);
        var resources = reports.Select(ReportResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("resources/{resourceId:int}")]
    public async Task<IActionResult> GetAllReportsByResourceId([FromRoute] int resourceId)
    {
        var getAllReportsByResourceIdQuery = new GetAllReportsByResourceIdQuery(resourceId);
        var reports = await reportQueryService.Handle(getAllReportsByResourceIdQuery);

        var resources = reports.Select(ReportResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get report by ID",
        Description = "Gets a specific report by its ID",
        OperationId = "GetReportById"
    )]
    [SwaggerResponse(200, "Report retrieved successfully", typeof(ReportResource))]
    [SwaggerResponse(404, "Report not found")]
    public async Task<IActionResult> GetReportById([FromRoute] int id)
    {
        var query = new GetReportByIdQuery(id);
        var report = await reportQueryService.Handle(query);

        if (report is null)
            return NotFound(new { Message = "Report not found." });

        var resource = ReportResourceFromEntityAssembler.ToResourceFromEntity(report);
        return Ok(resource);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update a report",
        Description = "Updates a report with the provided information",
        OperationId = "UpdateReport"
    )]
    [SwaggerResponse(200, "Report updated successfully", typeof(ReportResource))]
    [SwaggerResponse(404, "Report not found")]
    public async Task<IActionResult> UpdateReport([FromRoute] int id, [FromBody] UpdateReportResource resource)
    {
        try
        {
            var updateCommand = UpdateReportCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var updatedReport = await reportCommandService.Handle(updateCommand);

            if (updatedReport is null)
                return NotFound(new { Message = "Report not found." });

            var reportResource = ReportResourceFromEntityAssembler.ToResourceFromEntity(updatedReport);
            return Ok(reportResource);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a report",
        Description = "Deletes a report by its ID",
        OperationId = "DeleteReport"
    )]
    [SwaggerResponse(200, "Report deleted successfully")]
    [SwaggerResponse(404, "Report not found")]
    public async Task<IActionResult> DeleteReport([FromRoute] int id)
    {
        try
        {
            var deleteCommand = new DeleteReportCommand(id);
            await reportCommandService.Handle(deleteCommand);
            return Ok(new { Message = $"Report with ID {id} was deleted successfully." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}