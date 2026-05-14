using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST;

[ApiController]
[Authorize]
[Route("api/v1/")]
[Produces(MediaTypeNames.Application.Json)]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingCommandService meetingCommandService;
    private readonly IMeetingQueryService meetingQueryService;

    public MeetingsController(IMeetingCommandService meetingCommandService, IMeetingQueryService meetingQueryService)
    {
        this.meetingCommandService = meetingCommandService;
        this.meetingQueryService = meetingQueryService;
    }

    [HttpPost("administrators/{administratorId:int}/classrooms/{classroomId:int}/meetings")]
    [SwaggerOperation(
        Summary = "Creates a meeting",
        Description = "Creates a new meeting with specified details",
        OperationId = "CreateMeeting"
    )]
    [SwaggerResponse(201, "The meeting was created", typeof(MeetingResource))]
    [SwaggerResponse(400, "Validation error or referenced entity not found")]
    [SwaggerResponse(409, "Schedule conflict")]
    public async Task<IActionResult> CreateMeeting([FromRoute] int administratorId, [FromRoute] int classroomId,
        [FromBody] CreateMeetingResource resource)
    {
        var createMeetingCommand =
            CreateMeetingCommandFromResourceAssembler.ToCommandFromResource(administratorId, classroomId, resource);
        var meeting = await meetingCommandService.Handle(createMeetingCommand);
        if (meeting is null) return BadRequest("Failed to create meeting.");
        var meetingResource = MeetingResourceFromEntityAssembler.ToResourceFromEntity(meeting);
        return CreatedAtAction(nameof(GetMeetingById), new { id = meeting.Id }, meetingResource);
    }

    [HttpGet("meetings")]
    [SwaggerOperation(
        Summary = "Gets all meetings",
        Description = "Retrieves a list of all meetings",
        OperationId = "GetAllMeetings"
    )]
    public async Task<IActionResult> GetAllMeetings()
    {
        var getAllMeetingsQuery = new GetAllMeetingsQuery();
        var meetings = await meetingQueryService.Handle(getAllMeetingsQuery);
        var resources = meetings.Select(MeetingResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("administrators/{adminId:int}/meetings")]
    [SwaggerOperation(
        Summary = "Gets all meetings for an administrator",
        Description = "Retrieves a list of all meetings for a specific administrator",
        OperationId = "GetAllMeetingsForAdmin"
    )]
    public async Task<IActionResult> GetAllMeetingsForAdmin([FromRoute] int adminId)
    {
        var query = new GetAllMeetingByAdminIdQuery(adminId);
        var meetings = await meetingQueryService.Handle(query);
        var resources = meetings.Select(MeetingResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("teachers/{teacherId:int}/meetings")]
    [SwaggerOperation(
        Summary = "Gets all meetings for a teacher",
        Description = "Retrieves a list of all meetings for a specific teacher",
        OperationId = "GetAllMeetingsForTeacher"
    )]
    public async Task<IActionResult> GetAllMeetingsForTeacher([FromRoute] int teacherId)
    {
        var getAllMeetingsByTeacherIdQuery = new GetAllMeetingByTeacherIdQuery(teacherId);
        var meetings = await meetingQueryService.Handle(getAllMeetingsByTeacherIdQuery);
        var resources = meetings.Select(MeetingResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("meetings/{id:int}")]
    [SwaggerOperation(
        Summary = "Get meeting by ID",
        Description = "Gets a specific meeting by its ID",
        OperationId = "GetMeetingById"
    )]
    [SwaggerResponse(200, "Meeting retrieved successfully", typeof(MeetingResource))]
    [SwaggerResponse(404, "Meeting not found")]
    public async Task<IActionResult> GetMeetingById([FromRoute] int id)
    {
        var query = new GetMeetingByIdQuery(id);
        var meetings = await meetingQueryService.Handle(query);
        var meeting = meetings.FirstOrDefault();

        if (meeting is null)
            return NotFound(new { Message = $"Meeting with ID {id} was not found." });

        var resource = MeetingResourceFromEntityAssembler.ToResourceFromEntity(meeting);
        return Ok(resource);
    }

    [HttpPut("meetings/{id:int}")]
    [SwaggerOperation(
        Summary = "Updates a meeting",
        Description = "Updates a meeting by its ID with the provided details",
        OperationId = "UpdateMeeting"
    )]
    [SwaggerResponse(200, "The meeting was updated successfully", typeof(MeetingResource))]
    [SwaggerResponse(404, "The meeting was not found")]
    [SwaggerResponse(409, "Schedule conflict")]
    public async Task<IActionResult> UpdateMeeting([FromRoute] int id, [FromBody] UpdateMeetingResource resource)
    {
        var updateMeetingCommand = UpdateMeetingCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var updatedMeeting = await meetingCommandService.Handle(updateMeetingCommand);

        if (updatedMeeting is null)
            return NotFound(new { Message = $"Meeting with ID {id} was not found." });

        var meetingResource = MeetingResourceFromEntityAssembler.ToResourceFromEntity(updatedMeeting);
        return Ok(meetingResource);
    }

    [HttpDelete("meetings/{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes a meeting",
        Description = "Deletes the meeting specified by its ID",
        OperationId = "DeleteMeeting"
    )]
    [SwaggerResponse(204, "The meeting was deleted successfully.")]
    [SwaggerResponse(404, "Meeting not found.")]
    public async Task<IActionResult> DeleteMeeting([FromRoute] int id)
    {
        var deleteMeetingCommand = new DeleteMeetingCommand(id);
        await meetingCommandService.Handle(deleteMeetingCommand);
        return NoContent();
    }
}
