using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Interface.REST.Resources;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Interface.REST.Transform;
using FULLSTACKFURY.EduSpace.API.Reservations.Interface.REST.Resources;
using FULLSTACKFURY.EduSpace.API.Reservations.Interface.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.Reservations.Interface.REST;

[ApiController]
[Route("api/v1/")]
[Produces(MediaTypeNames.Application.Json)]
public class ReservationsController(
    IReservationCommandService reservationCommandService,
    IReservationQueryService reservationQueryService)
    : ControllerBase
{
    [HttpPost("teachers/{teacherId:int}/areas/{areaId:int}/reservations")]
    [SwaggerOperation(
        Summary = "Creates a reservation",
        Description = "Creates a reservation to a specific area",
        OperationId = "CreateReservation"
    )]
    [SwaggerResponse(201, "The category was created", typeof(ReservationResource))]
    public async Task<IActionResult> CreateReservation([FromRoute] int teacherId, [FromRoute] int areaId,
        [FromBody] CreateReservationResource resource)
    {
        var createReservationCommand =
            CreateReservationCommandFromResourceAssembler.ToCommandFromResource(areaId, teacherId, resource);
        var reservation = await reservationCommandService.Handle(createReservationCommand);

        if (reservation is null) return BadRequest();

        var reservationResource = ReservationResourceFromEntityAssembler.ToResourceFromEntity(reservation);
        return Ok(reservationResource);
    }

    [HttpGet("[controller]")]
    public async Task<IActionResult> GetAllReservations()
    {
        var getAllReservationsQuery = new GetAllReservationsQuery();
        var reservations = await reservationQueryService.Handle(getAllReservationsQuery);
        var resources = reservations.Select(ReservationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("areas/{areaId:int}/[controller]")]
    public async Task<IActionResult> GetAllReservationsByAreaId([FromRoute] int areaId)
    {
        var getAllReservationsByAreaIdQuery = new GetAllReservationsByAreaIdQuery(areaId);
        var reservations = await reservationQueryService.Handle(getAllReservationsByAreaIdQuery);

        var resources = reservations.Select(ReservationResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("[controller]/{id:int}")]
    [SwaggerOperation(
        Summary = "Get reservation by ID",
        Description = "Gets a specific reservation by its ID",
        OperationId = "GetReservationById"
    )]
    [SwaggerResponse(200, "Reservation retrieved successfully", typeof(ReservationResource))]
    [SwaggerResponse(404, "Reservation not found")]
    public async Task<IActionResult> GetReservationById([FromRoute] int id)
    {
        var query = new GetReservationByIdQuery(id);
        var reservation = await reservationQueryService.Handle(query);

        if (reservation is null)
            return NotFound(new { Message = "Reservation not found." });

        var resource = ReservationResourceFromEntityAssembler.ToResourceFromEntity(reservation);
        return Ok(resource);
    }

    [HttpPut("[controller]/{id:int}")]
    [SwaggerOperation(
        Summary = "Update a reservation",
        Description = "Updates a reservation with the provided information",
        OperationId = "UpdateReservation"
    )]
    [SwaggerResponse(200, "Reservation updated successfully", typeof(ReservationResource))]
    [SwaggerResponse(404, "Reservation not found")]
    public async Task<IActionResult> UpdateReservation([FromRoute] int id,
        [FromBody] UpdateReservationResource resource)
    {
        try
        {
            var updateCommand = UpdateReservationCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var updatedReservation = await reservationCommandService.Handle(updateCommand);

            if (updatedReservation is null)
                return NotFound(new { Message = "Reservation not found." });

            var reservationResource = ReservationResourceFromEntityAssembler.ToResourceFromEntity(updatedReservation);
            return Ok(reservationResource);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [HttpDelete("[controller]/{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a reservation",
        Description = "Deletes a reservation by its ID",
        OperationId = "DeleteReservation"
    )]
    [SwaggerResponse(200, "Reservation deleted successfully")]
    [SwaggerResponse(404, "Reservation not found")]
    public async Task<IActionResult> DeleteReservation([FromRoute] int id)
    {
        try
        {
            var deleteCommand = new DeleteReservationCommand(id);
            await reservationCommandService.Handle(deleteCommand);
            return Ok(new { Message = $"Reservation with ID {id} was deleted successfully." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}