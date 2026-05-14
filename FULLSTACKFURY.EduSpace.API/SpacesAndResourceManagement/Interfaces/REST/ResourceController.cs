using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Resource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST;

[ApiController]
[Authorize]
[Route("api/v1/classrooms/{classroomId:int}/resources")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Classrooms / Resources")]
public class ResourcesController(
    IResourceCommandService resourceCommandService,
    IResourceQueryService resourceQueryService) : ControllerBase
{
    /// <summary>Creates a new resource within a specific classroom.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateResource([FromRoute] int classroomId,
        [FromBody] CreateResourceResource resource)
    {
        var command = CreateResourceCommandFromResourceAssembler.ToCommandFromResource(classroomId, resource);
        var newResource = await resourceCommandService.Handle(command);
        if (newResource is null) return BadRequest();
        var resourceDto = ResourceResourceFromEntityAssembler.ToResourceFromEntity(newResource);
        return CreatedAtAction(nameof(GetResourceById),
            new { classroomId = newResource.ClassroomId, resourceId = newResource.Id }, resourceDto);
    }

    /// <summary>Gets all resources for a specific classroom.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllResourcesByClassroomId([FromRoute] int classroomId)
    {
        var query = new GetAllResourcesByClassroomIdQuery(classroomId);
        var resources = await resourceQueryService.Handle(query);
        return Ok(resources.Select(ResourceResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a specific resource by its ID from a specific classroom.</summary>
    [HttpGet("{resourceId:int}")]
    public async Task<IActionResult> GetResourceById([FromRoute] int classroomId, [FromRoute] int resourceId)
    {
        var query = new GetResourceByIdQuery(resourceId);
        var resource = await resourceQueryService.Handle(query);
        if (resource is null || resource.ClassroomId != classroomId) return NotFound();
        return Ok(ResourceResourceFromEntityAssembler.ToResourceFromEntity(resource));
    }

    /// <summary>Updates an existing resource.</summary>
    [HttpPut("{resourceId:int}")]
    public async Task<IActionResult> UpdateResource([FromRoute] int resourceId,
        [FromBody] UpdateResourceResource resource)
    {
        try
        {
            var command = UpdateResourceCommandFromResourceAssembler.ToCommandFromResource(resourceId, resource);
            var updatedResource = await resourceCommandService.Handle(command);
            if (updatedResource is null) return BadRequest("Could not update resource.");
            return Ok(ResourceResourceFromEntityAssembler.ToResourceFromEntity(updatedResource));
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>Deletes a resource by its ID.</summary>
    [HttpDelete("{resourceId:int}")]
    public async Task<IActionResult> DeleteResource([FromRoute] int resourceId)
    {
        try
        {
            await resourceCommandService.Handle(new DeleteResourceCommand(resourceId));
            return NoContent();
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}
