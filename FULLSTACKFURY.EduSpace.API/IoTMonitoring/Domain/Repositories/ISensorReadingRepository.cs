using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Repositories;

public interface ISensorReadingRepository : IBaseRepository<SensorReading>
{
    Task<IEnumerable<SensorReading>> FindAllAsync();
    Task<IEnumerable<SensorReading>> FindAllByDeviceIdAsync(string deviceId);
    Task<IEnumerable<SensorReading>> FindAllByZoneIdAsync(string zoneId);
    Task<SensorReading?> FindLatestByZoneIdAsync(string zoneId);
    // Idempotency key: a reading is uniquely identified by its edge node + edge-local id.
    // reading_id is only unique within one edge, so device_id is part of the key.
    Task<bool> ExistsByDeviceIdAndEdgeReadingIdAsync(string deviceId, int edgeReadingId);
}
