using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Repositories;

public interface ISensorReadingRepository : IBaseRepository<SensorReading>
{
    Task<IEnumerable<SensorReading>> FindAllAsync();
    Task<IEnumerable<SensorReading>> FindAllByDeviceIdAsync(string deviceId);
    Task<IEnumerable<SensorReading>> FindAllByZoneIdAsync(string zoneId);
    Task<SensorReading?> FindLatestByZoneIdAsync(string zoneId);
    Task<bool> ExistsByEdgeReadingIdAsync(string edgeReadingId);
}
