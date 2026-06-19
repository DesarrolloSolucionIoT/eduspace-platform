using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Services;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SensorReadingsController(
    ISensorReadingCommandService commandService,
    ISensorReadingQueryService queryService,
    ILogger<SensorReadingsController> logger)
    : ControllerBase
{
    /// <summary>
    /// Internal endpoint consumed by the Edge API forwarder (no JWT required).
    /// Receives the exact payload the Flask upstream_forwarder.py sends.
    /// </summary>
    [HttpPost("ingest")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Ingest a sensor reading from the Edge API",
        Description = "Receives a forwarded sensor reading from the ESP32 Edge gateway. Idempotent: duplicate edge reading IDs are silently ignored.",
        OperationId = "IngestSensorReading")]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Ingest([FromBody] IngestSensorReadingResource resource)
    {
        var command = IngestSensorReadingCommandFromResourceAssembler.ToCommandFromResource(resource);
        var reading = await commandService.Handle(command);

        if (reading is null)
            return Ok(new { message = "Duplicate reading ignored." });

        var readingResource = SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(reading);
        return CreatedAtAction(nameof(GetById), new { id = reading.Id }, readingResource);
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation(Summary = "Get all sensor readings", OperationId = "GetAllSensorReadings")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var readings = await queryService.Handle(new GetAllSensorReadingsQuery());
        return Ok(readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("device/{deviceId}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get sensor readings by device ID", OperationId = "GetSensorReadingsByDevice")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByDevice([FromRoute] string deviceId)
    {
        var readings = await queryService.Handle(new GetSensorReadingsByDeviceIdQuery(deviceId));
        return Ok(readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get sensor reading by ID", OperationId = "GetSensorReadingById")]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var all = await queryService.Handle(new GetAllSensorReadingsQuery());
        var reading = all.FirstOrDefault(r => r.Id == id);
        if (reading is null) return NotFound(new { message = "Sensor reading not found." });
        return Ok(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(reading));
    }

    [HttpGet("zone/{zoneId}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get sensor readings by zone ID", OperationId = "GetSensorReadingsByZone")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByZone([FromRoute] string zoneId)
    {
        var readings = await queryService.Handle(new GetSensorReadingsByZoneIdQuery(zoneId));
        return Ok(readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("zone/{zoneId}/latest")]
    [Authorize]
    [SwaggerOperation(Summary = "Get the latest sensor reading for a zone", OperationId = "GetLatestSensorReadingByZone")]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestByZone([FromRoute] string zoneId)
    {
        var reading = await queryService.HandleLatest(new GetSensorReadingsByZoneIdQuery(zoneId));
        if (reading is null) return NotFound(new { message = "No readings found for zone." });
        return Ok(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(reading));
    }
}
