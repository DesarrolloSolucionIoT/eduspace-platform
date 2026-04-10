using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST;

[ApiController]
[Route("api/v1/teachers-profiles")]
[Produces(MediaTypeNames.Application.Json)]
public class TeachersProfilesController(
    ITeacherProfileCommandService teacherProfileCommandService,
    ITeacherQueryService teacherQueryService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTeacherProfile([FromBody] CreateTeacherProfileResource resource)
    {
        var createProfileCommand = CreateTeacherProfileCommandFromResourceAssembler.ToCommandFromResource(resource);
        var teacherProfile = await teacherProfileCommandService.Handle(createProfileCommand);
        if (teacherProfile is null) return BadRequest();
        var teacherProfileResource = TeacherProfileResourceFromEntityAssembler.ToResourceFromEntity(teacherProfile);
        return Ok(teacherProfileResource);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTeacherProfiles()
    {
        var teacherProfiles = await teacherQueryService.Handle(new GetAllTeachersProfileQuery());
        var teacherResources
            = teacherProfiles.Select(TeacherProfileResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(teacherResources);
    }

    [HttpGet("{teacherId:int}")]
    public async Task<IActionResult> GetTeacherProfileById([FromRoute] int teacherId)
    {
        var teacherProfile = await teacherQueryService.Handle(new GetTeacherProfileByIdQuery(teacherId));
        if (teacherProfile is null) return NotFound();
        var teacherResource = TeacherProfileResourceFromEntityAssembler.ToResourceFromEntity(teacherProfile);
        return Ok(teacherResource);
    }

    [HttpPut("{teacherId:int}")]
    [SwaggerOperation(
        Summary = "Update a teacher profile",
        Description = "Updates the teacher profile with the provided information",
        OperationId = "UpdateTeacherProfile"
    )]
    [SwaggerResponse(200, "Teacher profile updated successfully", typeof(TeacherProfileResource))]
    [SwaggerResponse(404, "Teacher profile not found")]
    public async Task<IActionResult> UpdateTeacherProfile([FromRoute] int teacherId,
        [FromBody] UpdateTeacherProfileResource resource)
    {
        try
        {
            var updateCommand =
                UpdateTeacherProfileCommandFromResourceAssembler.ToCommandFromResource(teacherId, resource);
            var updatedProfile = await teacherProfileCommandService.Handle(updateCommand);

            if (updatedProfile is null)
                return NotFound(new { Message = "Teacher profile not found." });

            var profileResource = TeacherProfileResourceFromEntityAssembler.ToResourceFromEntity(updatedProfile);
            return Ok(profileResource);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpDelete("{teacherId:int}")]
    [SwaggerOperation(
        Summary = "Delete a teacher profile",
        Description = "Deletes the teacher profile with the specified ID",
        OperationId = "DeleteTeacherProfile"
    )]
    [SwaggerResponse(200, "Teacher profile deleted successfully")]
    [SwaggerResponse(404, "Teacher profile not found")]
    public async Task<IActionResult> DeleteTeacherProfile([FromRoute] int teacherId)
    {
        try
        {
            var deleteCommand = new DeleteTeacherProfileCommand(teacherId);
            await teacherProfileCommandService.Handle(deleteCommand);
            return Ok(new { Message = $"Teacher profile with ID {teacherId} was deleted successfully." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}