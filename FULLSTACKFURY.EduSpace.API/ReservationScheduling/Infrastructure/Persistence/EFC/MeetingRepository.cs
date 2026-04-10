using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Infrastructure.Persistence.EFC;

public class MeetingRepository(AppDbContext context) : BaseRepository<Meeting>(context), IMeetingRepository
{
    public override async Task<Meeting?> FindByIdAsync(int id)
    {
        return await Context.Set<Meeting>()
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.Teacher)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Meeting>> FindAllByAdminIdAsync(int adminId)
    {
        return await Context.Set<Meeting>()
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.Teacher)
            .Where(m => m.AdministratorId.AdministratorIdentifier == adminId)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Meeting>> ListAsync()
    {
        return await Context.Set<Meeting>()
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.Teacher)
            .ToListAsync();
    }

    public async Task<IEnumerable<Meeting>> FindAllByTeacherIdAsync(int teacherId)
    {
        return await Context.Set<Meeting>()
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.Teacher)
            .Where(m => m.MeetingParticipants.Any(mp => mp.TeacherId == teacherId))
            .ToListAsync();
    }
}