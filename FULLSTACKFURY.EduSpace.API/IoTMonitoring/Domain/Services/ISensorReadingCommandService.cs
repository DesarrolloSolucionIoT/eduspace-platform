using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Commands;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Services;

public interface ISensorReadingCommandService
{
    Task<SensorReading?> Handle(IngestSensorReadingCommand command);
}
