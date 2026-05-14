using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST;

[ApiController]
[Authorize]
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
    [SwaggerResponse(201, "Teacher added to meeting successfully")]
    [SwaggerResponse(400, "Invalid request — Meeting not found, Teacher not found, or Teacher already in meeting")]
    [SwaggerResponse(409, "Schedule conflict")]
    public async Task<IActionResult> AddTeacherToMeeting([FromRoute] int meetingId, [FromRoute] int teacherId)
    {
        // teacherId and meetingId come from route — no resource body needed.
        var command = new AddTeacherToMeetingCommand(teacherId, meetingId);
        await commandService.Handle(command);
        // 201 Created with no body — location header not applicable for participant sub-resource.
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpDelete]
    [SwaggerOperation(
        Summary = "Remove teacher from meeting",
        Description = "Removes a teacher from a meeting's participant list",
        OperationId = "RemoveTeacherFromMeeting")]
    [SwaggerResponse(204, "Teacher removed from meeting successfully")]
    [SwaggerResponse(400, "Invalid request — Meeting not found or Teacher not found")]
    [SwaggerResponse(404, "Teacher not associated with this meeting")]
    public async Task<IActionResult> RemoveTeacherFromMeeting([FromRoute] int meetingId, [FromRoute] int teacherId)
    {
        var command = new RemoveTeacherFromMeetingCommand(teacherId, meetingId);
        await commandService.Handle(command);
        return NoContent();
    }
}
