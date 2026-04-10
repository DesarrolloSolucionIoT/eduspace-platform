using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST;

[ApiController]
[Route("api/v1/meetings/{meetingId:int}/teachers/{teacherId:int}")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Meetings")]
public class MeetingParticipantsController(IMeetingCommandService commandService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Add teacher to meeting",
        Description = "Adds a teacher to a meeting's participant list",
        OperationId = "AddTeacherToMeeting")]
    [SwaggerResponse(200, "Teacher added to meeting successfully")]
    [SwaggerResponse(400, "Invalid request - Meeting not found, Teacher not found, or Teacher already in meeting")]
    public async Task<IActionResult> AddTeacherToMeeting([FromRoute] int meetingId, [FromRoute] int teacherId)
    {
        var addTeacherToMeetingResource = new AddTeacherToMeetingResource(teacherId, meetingId);
        var addTeacherToMeetingCommand = AddTeacherToMeetingCommandFromResourceAssembler
            .ToCommandFromResource(addTeacherToMeetingResource);
        await commandService.Handle(addTeacherToMeetingCommand);
        return Ok("Teacher added to meeting.");
    }

    [HttpDelete("")]
    [SwaggerOperation(
        Summary = "Remove teacher from meeting",
        Description = "Removes a teacher from a meeting's participant list",
        OperationId = "RemoveTeacherFromMeeting")]
    [SwaggerResponse(200, "Teacher removed from meeting successfully")]
    [SwaggerResponse(400,
        "Invalid request - Meeting not found, Teacher not found, or Teacher not associated with meeting")]
    public async Task<IActionResult> RemoveTeacherFromMeeting([FromRoute] int meetingId, [FromRoute] int teacherId)
    {
        var removeTeacherFromMeetingCommand = new RemoveTeacherFromMeetingCommand(teacherId, meetingId);
        await commandService.Handle(removeTeacherFromMeetingCommand);
        return Ok("Teacher removed from meeting.");
    }
}