using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Resources;

/// <summary>
/// Matches the snake_case JSON payload that the Edge API (Flask) forwards.
/// </summary>
public record IngestSensorReadingResource(
    [property: JsonPropertyName("reading_id")]      int ReadingId,
    [property: JsonPropertyName("device_id")]       [Required] string DeviceId,
    [property: JsonPropertyName("zone_id")]         string? ZoneId,
    [property: JsonPropertyName("temperature")]     float Temperature,
    [property: JsonPropertyName("humidity")]        float Humidity,
    [property: JsonPropertyName("occupancy")]       bool Occupancy,
    [property: JsonPropertyName("alert_led_state")] int AlertLedState,
    [property: JsonPropertyName("recorded_at")]     DateTime RecordedAt);
