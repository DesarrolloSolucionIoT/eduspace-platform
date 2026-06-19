using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Transform;

public static class IngestSensorReadingCommandFromResourceAssembler
{
    public static IngestSensorReadingCommand ToCommandFromResource(IngestSensorReadingResource resource)
        => new(
            resource.ReadingId,
            resource.DeviceId,
            resource.ZoneId,
            resource.Temperature,
            resource.Humidity,
            resource.Occupancy,
            resource.AlertLedState,
            resource.RecordedAt);
}
