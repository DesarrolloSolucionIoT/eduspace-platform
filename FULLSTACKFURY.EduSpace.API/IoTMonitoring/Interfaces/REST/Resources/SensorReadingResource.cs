namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Resources;

public record SensorReadingResource(
    int Id,
    int EdgeReadingId,
    string DeviceId,
    string? ZoneId,
    float Temperature,
    float Humidity,
    bool OccupancyPresent,
    int AlertLedState,
    DateTime RecordedAt,
    DateTime ReceivedAt);
