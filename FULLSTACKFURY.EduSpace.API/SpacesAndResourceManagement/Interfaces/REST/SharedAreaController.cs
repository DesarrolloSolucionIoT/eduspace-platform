using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/shared-area")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Shared Areas Endpoints")]
public class SharedAreaController(
    ISharedAreaQueryService sharedAreaQueryService,
    ISharedAreaCommandService sharedAreaCommandService) : ControllerBase
{
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get a shared area by id",
        Description = "Get a shared area by id",
        OperationId = "GetSharedAreaById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "The shared area was successfully retrieved", typeof(SharedArea))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The shared area was not found")]
    public async Task<IActionResult> GetSharedAreaById(int id)
    {
        var getSharedAreaByIdQuery = new GetSharedAreaByIdQuery(id);
        var shareArea = await sharedAreaQueryService.Handle(getSharedAreaByIdQuery);
        if (shareArea is null) return NotFound();
        var sharedAreaResource = SharedAreaResourceFromEntityAssembler.ToResourceFromEntity(shareArea);
        return Ok(sharedAreaResource);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a shared area",
        Description = "Create a shared area",
        OperationId = "CreateSharedArea"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "The shared area was successfully created", typeof(SharedArea))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The shared area could not be created")]
    public async Task<IActionResult> CreateSharedArea([FromBody] CreateSharedAreaResource resource)
    {
        var createSharedAreaCommand = CreateSharedAreaCommandFromResourceAssembler.ToCommandFromResource(resource);
        var sharedArea = await sharedAreaCommandService.Handle(createSharedAreaCommand);
        if (sharedArea is null) return BadRequest();
        var sharedAreaResource = SharedAreaResourceFromEntityAssembler.ToResourceFromEntity(sharedArea);
        return CreatedAtAction(nameof(GetSharedAreaById), new { id = sharedArea.Id }, sharedAreaResource);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all shared areas",
        Description = "Get all shared areas",
        OperationId = "GetAllSharedAreas"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "The shared areas were successfully retrieved",
        typeof(IEnumerable<SharedArea>))]
    public async Task<IActionResult> GetAllSharedAreas()
    {
        var getAllSharedAreasQuery = new GetAllSharedAreasQuery();
        var sharedAreas = await sharedAreaQueryService.Handle(getAllSharedAreasQuery);
        var sharedAreaResources = sharedAreas.Select(SharedAreaResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(sharedAreaResources);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates a shared areas",
        Description = "Updates a shared areas  by its ID with the provided details",
        OperationId = "UpdateMeeting"
    )]
    [SwaggerResponse(200, "The shared area was updated successfully", typeof(SharedAreaResource))]
    [SwaggerResponse(404, "The shared area  was not found")]
    public async Task<IActionResult> UpdateSharedArea([FromRoute] int id, [FromBody] UpdateSharedAreaResource resource)
    {
        try
        {
            // Map the resource to the UpdateMeetingCommand
            var updateSharedAreaCommand =
                UpdateSharedAreaCommandFromResourceAssembler.ToCommandFromResource(id, resource);

            // Handle the update command
            var updatedSharedArea = await sharedAreaCommandService.Handle(updateSharedAreaCommand);

            // If the meeting was not updated, return not found
            if (updatedSharedArea is null)
                return NotFound(new { Message = "Meeting not found." });

            // Map the updated meeting entity to the resource
            var meetingResource = SharedAreaResourceFromEntityAssembler.ToResourceFromEntity(updatedSharedArea);
            return Ok(meetingResource);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes a shared areas",
        Description = "Deletes the shared areas specified by its ID",
        OperationId = "DeleteMeeting"
    )]
    [SwaggerResponse(200, "The shared area was deleted successfully.")]
    [SwaggerResponse(404, "Shared area not found.")]
    public async Task<IActionResult> DeleteSharedArea([FromRoute] int id)
    {
        try
        {
            var deleteSharedAreaCommand = new DeleteSharedAreaCommand(id);
            await sharedAreaCommandService.Handle(deleteSharedAreaCommand);
            return Ok($"Shared area with ID {id} was deleted successfully.");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}