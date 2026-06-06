using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Infrastructure.Persistence.EFC.Repositories;

public class SensorReadingRepository(AppDbContext context)
    : BaseRepository<SensorReading>(context), ISensorReadingRepository
{
    public override async Task<SensorReading?> FindByIdAsync(int id)
        => await Context.Set<SensorReading>().FindAsync(id);

    public async Task<IEnumerable<SensorReading>> FindAllAsync()
        => await Context.Set<SensorReading>()
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync();

    public async Task<IEnumerable<SensorReading>> FindAllByDeviceIdAsync(string deviceId)
        => await Context.Set<SensorReading>()
            .Where(r => r.DeviceId == deviceId)
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync();

    public async Task<IEnumerable<SensorReading>> FindAllByZoneIdAsync(string zoneId)
        => await Context.Set<SensorReading>()
            .Where(r => r.ZoneId == zoneId)
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync();

    public async Task<SensorReading?> FindLatestByZoneIdAsync(string zoneId)
        => await Context.Set<SensorReading>()
            .Where(r => r.ZoneId == zoneId)
            .OrderByDescending(r => r.RecordedAt)
            .FirstOrDefaultAsync();

    public async Task<bool> ExistsByEdgeReadingIdAsync(string edgeReadingId)
        => await Context.Set<SensorReading>()
            .AnyAsync(r => r.EdgeReadingId == edgeReadingId);
}
