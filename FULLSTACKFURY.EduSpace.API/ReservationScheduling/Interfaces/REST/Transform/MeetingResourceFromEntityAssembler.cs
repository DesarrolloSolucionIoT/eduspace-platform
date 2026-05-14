using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Transform;

// TODO[ACL]: teacher name is returned as "Unknown" until GetTeacherSummaryAsync is
// implemented in ExternalProfileService and wired here — see Fase 5, issue #2.
// At that point, inject IExternalProfileService and call it per participant.
public class MeetingResourceFromEntityAssembler
{
    public static MeetingResource ToResourceFromEntity(Meeting entity)
    {
        var teachers = entity.MeetingParticipants
            .Select(mp => new TeacherResource(
                mp.TeacherId,
                "Unknown",  // TODO[ACL]: replace with ACL summary fetch (Fase 5)
                ""
            ))
            .ToList();

        return new MeetingResource(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Date,
            entity.StartTime,
            entity.EndTime,
            entity.AdministratorId,
            entity.ClassroomId,
            teachers
        );
    }
}
