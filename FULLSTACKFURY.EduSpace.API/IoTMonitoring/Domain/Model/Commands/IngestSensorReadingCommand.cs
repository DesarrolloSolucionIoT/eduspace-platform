namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Commands;

public record IngestSensorReadingCommand(
    int EdgeReadingId,
    string DeviceId,
    string? ZoneId,
    float Temperature,
    float Humidity,
    bool OccupancyPresent,
    int AlertLedState,
    DateTime RecordedAt);
