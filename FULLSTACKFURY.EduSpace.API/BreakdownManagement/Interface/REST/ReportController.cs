using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Resources;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class ReportsController(
    IReportCommandService reportCommandService,
    IReportQueryService reportQueryService,
    ILogger<ReportsController> logger)
    : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a report",
        Description = "Creates a report for a specific resource",
        OperationId = "CreateReport"
    )]
    [ProducesResponseType(typeof(ReportResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportResource resource)
    {
        var createReportCommand = CreateReportCommandFromResourceAssembler.ToCommandFromResource(resource);
        var report = await reportCommandService.Handle(createReportCommand);

        if (report is null) return BadRequest();

        var reportResource = ReportResourceFromEntityAssembler.ToResourceFromEntity(report);
        return CreatedAtAction(nameof(GetReportById), new { id = report.Id }, reportResource);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllReports()
    {
        var getAllReportsQuery = new GetAllReportsQuery();
        var reports = await reportQueryService.Handle(getAllReportsQuery);
        var resources = reports.Select(ReportResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("resources/{resourceId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ReportResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(typeof(ReportResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        Description = "Updates a report content and optionally transitions its status. " +
                      "Allowed status values: \"in progress\", \"completed\".",
        OperationId = "UpdateReport"
    )]
    [ProducesResponseType(typeof(ReportResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReport([FromRoute] int id, [FromBody] UpdateReportResource resource)
    {
        var updateCommand = UpdateReportCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var updatedReport = await reportCommandService.Handle(updateCommand);

        if (updatedReport is null)
            return NotFound(new { Message = "Report not found." });

        var reportResource = ReportResourceFromEntityAssembler.ToResourceFromEntity(updatedReport);
        return Ok(reportResource);
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a report",
        Description = "Deletes a report by its ID",
        OperationId = "DeleteReport"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReport([FromRoute] int id)
    {
        var deleteCommand = new DeleteReportCommand(id);
        await reportCommandService.Handle(deleteCommand);
        return NoContent();
    }
}
