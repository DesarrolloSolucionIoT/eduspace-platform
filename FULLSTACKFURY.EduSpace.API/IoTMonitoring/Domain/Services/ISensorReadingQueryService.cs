using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Services;


public interface ISensorReadingQueryService
{
    Task<IEnumerable<SensorReading>> Handle(GetAllSensorReadingsQuery query);
    Task<IEnumerable<SensorReading>> Handle(GetSensorReadingsByDeviceIdQuery query);
    Task<IEnumerable<SensorReading>> Handle(GetSensorReadingsByZoneIdQuery query);
    Task<SensorReading?> HandleLatest(GetSensorReadingsByZoneIdQuery query);
}
