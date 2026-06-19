using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Application.Internal.CommandServices;

public class SensorReadingCommandService(
    ISensorReadingRepository sensorReadingRepository,
    IUnitOfWork unitOfWork,
    ILogger<SensorReadingCommandService> logger)
    : ISensorReadingCommandService
{
    public async Task<SensorReading?> Handle(IngestSensorReadingCommand command)
    {
        // Deduplicate: drop readings already persisted from the same Edge reading ID.
        if (await sensorReadingRepository.ExistsByEdgeReadingIdAsync(command.EdgeReadingId))
        {
            logger.LogInformation("Duplicate edge reading {EdgeReadingId} ignored.", command.EdgeReadingId);
            return null;
        }

        var reading = new SensorReading(command);
        await sensorReadingRepository.AddAsync(reading);
        await unitOfWork.CompleteAsync();

        logger.LogInformation("SensorReading {Id} ingested from device {DeviceId}.", reading.Id, command.DeviceId);
        return reading;
    }
}
