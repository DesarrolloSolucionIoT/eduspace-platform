using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Resource;
using Microsoft.AspNetCore.Mvc;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/classrooms/{classroomId:int}/resources")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Classrooms / Resources")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceCommandService _resourceCommandService;
    private readonly IResourceQueryService _resourceQueryService;

    public ResourcesController(IResourceCommandService resourceCommandService,
        IResourceQueryService resourceQueryService)
    {
        _resourceCommandService = resourceCommandService;
        _resourceQueryService = resourceQueryService;
    }

    /// <summary>
    ///     Creates a new resource within a specific classroom.
    /// </summary>
    [HttpPost] // La ruta es simplemente la base: .../{classroomId}/resources
    public async Task<IActionResult> CreateResource([FromRoute] int classroomId,
        [FromBody] CreateResourceResource resource)
    {
        var command = CreateResourceCommandFromResourceAssembler.ToCommandFromResource(classroomId, resource);
        var newResource = await _resourceCommandService.Handle(command);
        if (newResource is null) return BadRequest();

        var resourceDto = ResourceResourceFromEntityAssembler.ToResourceFromEntity(newResource);

        // ✨ 2. CORRECCIÓN: Pasamos ambos parámetros de ruta para que CreatedAtAction funcione.
        return CreatedAtAction(nameof(GetResourceById),
            new { classroomId = newResource.ClassroomId, resourceId = newResource.Id }, resourceDto);
    }

    /// <summary>
    ///     Gets all resources for a specific classroom.
    /// </summary>
    [HttpGet] // La ruta es: .../{classroomId}/resources
    public async Task<IActionResult> GetAllResourcesByClassroomId([FromRoute] int classroomId)
    {
        var query = new GetAllResourcesByClassroomIdQuery(classroomId);
        var resources = await _resourceQueryService.Handle(query);
        var resourceDtos = resources.Select(ResourceResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resourceDtos);
    }

    /// <summary>
    ///     Gets a specific resource by its ID from a specific classroom.
    /// </summary>
    [HttpGet("{resourceId:int}")] // La ruta es: .../{classroomId}/resources/{resourceId}
    public async Task<IActionResult> GetResourceById([FromRoute] int classroomId, [FromRoute] int resourceId)
    {
        var query = new GetResourceByIdQuery(resourceId);
        var resource = await _resourceQueryService.Handle(query);

        // Verificación de seguridad: el recurso debe pertenecer al aula especificada.
        if (resource == null || resource.ClassroomId != classroomId) return NotFound();

        var resourceDto = ResourceResourceFromEntityAssembler.ToResourceFromEntity(resource);
        return Ok(resourceDto);
    }

    /// <summary>
    ///     Updates an existing resource.
    /// </summary>
    [HttpPut("{resourceId:int}")] // La ruta es: .../{classroomId}/resources/{resourceId}
    public async Task<IActionResult> UpdateResource([FromRoute] int resourceId,
        [FromBody] UpdateResourceResource resource)
    {
        var command = UpdateResourceCommandFromResourceAssembler.ToCommandFromResource(resourceId, resource);
        var updatedResource = await _resourceCommandService.Handle(command);
        if (updatedResource == null) return BadRequest("Could not update resource.");

        var resourceDto = ResourceResourceFromEntityAssembler.ToResourceFromEntity(updatedResource);
        return Ok(resourceDto);
    }

    /// <summary>
    ///     Deletes a resource by its ID.
    /// </summary>
    [HttpDelete("{resourceId:int}")] // La ruta es: .../{classroomId}/resources/{resourceId}
    public async Task<IActionResult> DeleteResource([FromRoute] int resourceId)
    {
        var command = new DeleteResourceCommand(resourceId);
        await _resourceCommandService.Handle(command);
        return NoContent(); // Respuesta 204 No Content es estándar para DELETE exitoso.
    }
}