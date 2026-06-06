using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Transform;

public static class SensorReadingResourceFromEntityAssembler
{
    public static SensorReadingResource ToResourceFromEntity(SensorReading entity)
        => new(
            entity.Id,
            entity.EdgeReadingId,
            entity.DeviceId,
            entity.ZoneId,
            entity.Temperature,
            entity.Humidity,
            entity.OccupancyPresent,
            entity.AlertLedState,
            entity.RecordedAt,
            entity.ReceivedAt);
}
