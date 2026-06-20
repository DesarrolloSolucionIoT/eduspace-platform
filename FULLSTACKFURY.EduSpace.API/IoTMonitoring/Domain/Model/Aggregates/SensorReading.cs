using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Commands;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;

public class SensorReading
{
    // Parameterless constructor required by EF Core for entity rehydration.
    protected SensorReading() { }

    public SensorReading(IngestSensorReadingCommand command)
    {
        EdgeReadingId = command.EdgeReadingId;
        DeviceId = command.DeviceId;
        ZoneId = command.ZoneId;
        Temperature = command.Temperature;
        Humidity = command.Humidity;
        OccupancyPresent = command.OccupancyPresent;
        AlertLedState = command.AlertLedState;
        RecordedAt = command.RecordedAt;
        ReceivedAt = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int EdgeReadingId { get; private set; }
    public string DeviceId { get; private set; } = string.Empty;
    public string? ZoneId { get; private set; }
    public float Temperature { get; private set; }
    public float Humidity { get; private set; }
    public bool OccupancyPresent { get; private set; }
    public int AlertLedState { get; private set; }
    public DateTime RecordedAt { get; private set; }
    public DateTime ReceivedAt { get; private set; }
}
