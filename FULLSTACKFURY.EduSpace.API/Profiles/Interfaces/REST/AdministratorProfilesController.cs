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
[Route("api/v1/administrator-profiles")]
[Produces(MediaTypeNames.Application.Json)]
public class AdministratorProfilesController(
    IAdminProfileCommandService profileCommandService,
    IAdminProfileQueryService profileQueryService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAdministratorProfile([FromBody] CreateAdminProfileResource resource)
    {
        var createAdminProfileCommand = CreateAdminProfileCommandFromResourceAssembler.ToCommandFromResource(resource);
        var adminProfile = await profileCommandService.Handle(createAdminProfileCommand);

        if (adminProfile is null) return BadRequest();

        var adminProfileResource = AdminProfileResourceFromEntityAssembler.ToResourceFromEntity(adminProfile);

        return Ok(adminProfileResource);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAdministratorProfiles()
    {
        var administratorProfiles = await profileQueryService.Handle(new GetAllAdministratorsProfileQuery());
        var administratorResources =
            administratorProfiles.Select(AdminProfileResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(administratorResources);
    }

    [HttpGet("{administratorId:int}")]
    public async Task<IActionResult> GetTeacherProfileById([FromRoute] int administratorId)
    {
        var administratorProfile =
            await profileQueryService.Handle(new GetAdministratorProfileByIdQuery(administratorId));
        if (administratorProfile is null) return NotFound();
        var administratorResource = AdminProfileResourceFromEntityAssembler.ToResourceFromEntity(administratorProfile);
        return Ok(administratorResource);
    }

    [HttpPut("{administratorId:int}")]
    [SwaggerOperation(
        Summary = "Update an administrator profile",
        Description = "Updates the administrator profile with the provided information",
        OperationId = "UpdateAdministratorProfile"
    )]
    [SwaggerResponse(200, "Administrator profile updated successfully", typeof(AdminProfileResource))]
    [SwaggerResponse(404, "Administrator profile not found")]
    public async Task<IActionResult> UpdateAdministratorProfile([FromRoute] int administratorId,
        [FromBody] UpdateAdminProfileResource resource)
    {
        try
        {
            var updateCommand =
                UpdateAdminProfileCommandFromResourceAssembler.ToCommandFromResource(administratorId, resource);
            var updatedProfile = await profileCommandService.Handle(updateCommand);

            if (updatedProfile is null)
                return NotFound(new { Message = "Administrator profile not found." });

            var profileResource = AdminProfileResourceFromEntityAssembler.ToResourceFromEntity(updatedProfile);
            return Ok(profileResource);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpDelete("{administratorId:int}")]
    [SwaggerOperation(
        Summary = "Delete an administrator profile",
        Description = "Deletes the administrator profile with the specified ID",
        OperationId = "DeleteAdministratorProfile"
    )]
    [SwaggerResponse(200, "Administrator profile deleted successfully")]
    [SwaggerResponse(404, "Administrator profile not found")]
    public async Task<IActionResult> DeleteAdministratorProfile([FromRoute] int administratorId)
    {
        try
        {
            var deleteCommand = new DeleteAdminProfileCommand(administratorId);
            await profileCommandService.Handle(deleteCommand);
            return Ok(new { Message = $"Administrator profile with ID {administratorId} was deleted successfully." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}