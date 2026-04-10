using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;

public interface IMeetingQueryService
{
    Task<IEnumerable<Meeting>> Handle(GetAllMeetingsQuery query);
    Task<IEnumerable<Meeting>> Handle(GetMeetingByIdQuery query);
    Task<IEnumerable<Meeting>> Handle(GetAllMeetingByAdminIdQuery query);
    Task<IEnumerable<Meeting>> Handle(GetAllMeetingByTeacherIdQuery query);
}