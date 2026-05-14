using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Infrastructure.Persistence.EFC;

public class MeetingRepository(AppDbContext context) : BaseRepository<Meeting>(context), IMeetingRepository
{
    // NOTE: .Teacher navigation was removed — ACL fix #2. Only TeacherId FK remains on MeetingSession.
    // Teacher name resolution goes through IExternalProfileService (Profiles ACL).

    public override async Task<Meeting?> FindByIdAsync(int id)
    {
        return await Context.Set<Meeting>()
            .Include(m => m.MeetingParticipants)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Meeting>> FindAllByAdminIdAsync(int adminId)
    {
        return await Context.Set<Meeting>()
            .AsNoTracking()
            .Include(m => m.MeetingParticipants)
            .Where(m => m.AdministratorId.AdministratorIdentifier == adminId)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Meeting>> ListAsync()
    {
        return await Context.Set<Meeting>()
            .AsNoTracking()
            .Include(m => m.MeetingParticipants)
            .ToListAsync();
    }

    public async Task<IEnumerable<Meeting>> FindAllByTeacherIdAsync(int teacherId)
    {
        return await Context.Set<Meeting>()
            .AsNoTracking()
            .Include(m => m.MeetingParticipants)
            .Where(m => m.MeetingParticipants.Any(mp => mp.TeacherId == teacherId))
            .ToListAsync();
    }
}
