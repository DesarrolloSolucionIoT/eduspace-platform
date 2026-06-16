using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST;

[ApiController]
[Authorize]
[Route("api/v1/shared-area")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Shared Areas Endpoints")]
public class SharedAreaController(
    ISharedAreaQueryService sharedAreaQueryService,
    ISharedAreaCommandService sharedAreaCommandService
    ) : ControllerBase
{
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get a shared area by id", OperationId = "GetSharedAreaById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The shared area was successfully retrieved", typeof(SharedAreaResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The shared area was not found")]
    public async Task<IActionResult> GetSharedAreaById(int id)
    {
        var query = new GetSharedAreaByIdQuery(id);
        var sharedArea = await sharedAreaQueryService.Handle(query);
        if (sharedArea is null) return NotFound();
        return Ok(SharedAreaResourceFromEntityAssembler.ToResourceFromEntity(sharedArea));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a shared area", OperationId = "CreateSharedArea")]
    [SwaggerResponse(StatusCodes.Status201Created, "The shared area was successfully created", typeof(SharedAreaResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The shared area could not be created")]
    public async Task<IActionResult> CreateSharedArea([FromBody] CreateSharedAreaResource resource)
    {
        var command = CreateSharedAreaCommandFromResourceAssembler.ToCommandFromResource(resource);
        var sharedArea = await sharedAreaCommandService.Handle(command);
        if (sharedArea is null) return BadRequest();
        var sharedAreaResource = SharedAreaResourceFromEntityAssembler.ToResourceFromEntity(sharedArea);
        return CreatedAtAction(nameof(GetSharedAreaById), new { id = sharedArea.Id }, sharedAreaResource);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all shared areas", OperationId = "GetAllSharedAreas")]
    [SwaggerResponse(StatusCodes.Status200OK, "The shared areas were successfully retrieved",
        typeof(IEnumerable<SharedAreaResource>))]
    public async Task<IActionResult> GetAllSharedAreas()
    {
        var sharedAreas = await sharedAreaQueryService.Handle(new GetAllSharedAreasQuery());
        return Ok(sharedAreas.Select(SharedAreaResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update a shared area", OperationId = "UpdateSharedArea")]
    [SwaggerResponse(200, "The shared area was updated successfully", typeof(SharedAreaResource))]
    [SwaggerResponse(404, "The shared area was not found")]
    public async Task<IActionResult> UpdateSharedArea([FromRoute] int id, [FromBody] UpdateSharedAreaResource resource)
    {
        try
        {
            // Map the resource to the UpdateSharedAreaCommand
            var command = UpdateSharedAreaCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var updatedSharedArea = await sharedAreaCommandService.Handle(command);

            // If the shared area was not updated, return not found
            if (updatedSharedArea is null)
                return NotFound(new { Message = "Shared area not found." });

            // Map the updated shared area entity to the resource
            var sharedAreaResource = SharedAreaResourceFromEntityAssembler.ToResourceFromEntity(updatedSharedArea);
            return Ok(sharedAreaResource);
        }
        catch (SharedAreaNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a shared area", OperationId = "DeleteSharedArea")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The shared area was deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Shared area not found.")]
    public async Task<IActionResult> DeleteSharedArea([FromRoute] int id)
    {
        try
        {
            await sharedAreaCommandService.Handle(new DeleteSharedAreaCommand(id));
            return NoContent();
        }
        catch (SharedAreaNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    // ── Reservation endpoints ───────────────────────────────────────────────

    [HttpPost("{id:int}/reserve")]
    [SwaggerOperation(Summary = "Reserve a shared area", OperationId = "ReserveSharedArea")]
    [SwaggerResponse(StatusCodes.Status201Created, "The reservation was created successfully",
        typeof(SharedAreaReservationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The reservation could not be created")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Shared area not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Time slot already reserved")]
    public async Task<IActionResult> ReserveSharedArea([FromRoute] int id,
        [FromBody] ReserveSharedAreaResource resource)
    {
        try
        {
            var command = ReserveSharedAreaCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var reservation = await sharedAreaCommandService.Handle(command);
            if (reservation is null) return BadRequest();
            var reservationResource =
                SharedAreaReservationResourceFromEntityAssembler.ToResourceFromEntity(reservation);
            return CreatedAtAction(nameof(GetReservationsBySharedAreaId),
                new { sharedAreaId = id }, reservationResource);
        }
        catch (SharedAreaNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (SharedAreaReservationConflictException ex)
        {
            return Conflict(new { ex.Message });
        }
    }

    [HttpGet("{id:int}/reservations")]
    [SwaggerOperation(Summary = "Get reservations for a shared area", OperationId = "GetReservationsBySharedAreaId")]
    [SwaggerResponse(StatusCodes.Status200OK, "The reservations were retrieved successfully",
        typeof(IEnumerable<SharedAreaReservationResource>))]
    public async Task<IActionResult> GetReservationsBySharedAreaId([FromRoute] int id, [FromQuery] DateTime date)
    {
        var query = new GetAllReservationsBySharedAreaIdQuery(id, DateOnly.FromDateTime(date));
        
        var reservations = await sharedAreaQueryService.Handle(query);

        return Ok(reservations.Select(SharedAreaReservationResourceFromEntityAssembler.ToResourceFromEntity));

    }

    [HttpGet("teacher/{teacherId:int}/reservations")]
    [SwaggerOperation(Summary = "Get reservations for a teacher", OperationId = "GetReservationsByTeacherId")]
    [SwaggerResponse(StatusCodes.Status200OK, "The reservations were retrieved successfully",
        typeof(IEnumerable<SharedAreaReservationResource>))]
    public async Task<IActionResult> GetReservationsByTeacherId([FromRoute] int teacherId)
    {
        var query = new GetAllReservationsByTeacherIdQuery(teacherId);
        var reservations = await sharedAreaQueryService.Handle(query);
        return Ok(reservations.Select(SharedAreaReservationResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
