using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Transform;

public static class IngestSensorReadingCommandFromResourceAssembler
{
    // The edge does not forward zone_id, but its device ids follow the convention
    // "esp32-<zone>" (e.g. "esp32-aula-101" → "aula-101"), the same convention used to
    // seed devices on the edge. We derive the zone from the device id when the payload
    // doesn't carry one, so the dashboard's Classroom.ZoneId == SensorReading.ZoneId link
    // keeps working with zero changes on the edge side.
    private const string DeviceIdPrefix = "esp32-";

    // Single-zone deployment fallback: with one device/classroom there is no zone_id in
    // the UI, so any reading that can't be resolved is tagged with this default zone.
    // Change this if the project's single classroom uses a different zone id.
    private const string DefaultZoneId = "aula-101";

    public static IngestSensorReadingCommand ToCommandFromResource(IngestSensorReadingResource resource)
        => new(
            resource.ReadingId,
            resource.DeviceId,
            ResolveZoneId(resource),
            resource.Temperature,
            resource.Humidity,
            resource.Occupancy,
            resource.AlertLedState,
            resource.RecordedAt);

    private static string? ResolveZoneId(IngestSensorReadingResource resource)
    {
        // Respect an explicit zone_id if the edge ever starts sending one.
        if (!string.IsNullOrWhiteSpace(resource.ZoneId))
            return resource.ZoneId;

        // Otherwise derive it from the device id convention "esp32-<zone>".
        var deviceId = resource.DeviceId;
        if (!string.IsNullOrWhiteSpace(deviceId) &&
            deviceId.StartsWith(DeviceIdPrefix, StringComparison.OrdinalIgnoreCase))
            return deviceId[DeviceIdPrefix.Length..];

        // Unknown convention → fall back to the single-zone default so the reading is
        // always queryable by zone (column stays nullable for safety, but we avoid null here).
        return DefaultZoneId;
    }
}
