using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Application.Internal.QueryServices;

public class SensorReadingQueryService(ISensorReadingRepository sensorReadingRepository)
    : ISensorReadingQueryService
{
    public async Task<IEnumerable<SensorReading>> Handle(GetAllSensorReadingsQuery query)
        => await sensorReadingRepository.FindAllAsync();

    public async Task<IEnumerable<SensorReading>> Handle(GetSensorReadingsByDeviceIdQuery query)
        => await sensorReadingRepository.FindAllByDeviceIdAsync(query.DeviceId);

    public async Task<IEnumerable<SensorReading>> Handle(GetSensorReadingsByZoneIdQuery query)
        => await sensorReadingRepository.FindAllByZoneIdAsync(query.ZoneId);

    public async Task<SensorReading?> HandleLatest(GetSensorReadingsByZoneIdQuery query)
        => await sensorReadingRepository.FindLatestByZoneIdAsync(query.ZoneId);
}
