using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;

public interface IMeetingRepository : IBaseRepository<Meeting>
{
    Task<IEnumerable<Meeting>> FindAllByAdminIdAsync(int adminId);
    Task<IEnumerable<Meeting>> FindAllByTeacherIdAsync(int teacherId);
}