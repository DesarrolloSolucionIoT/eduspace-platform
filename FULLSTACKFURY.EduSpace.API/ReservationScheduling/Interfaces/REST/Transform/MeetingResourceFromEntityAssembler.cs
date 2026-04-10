using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Transform;

public class MeetingResourceFromEntityAssembler
{
    public static MeetingResource ToResourceFromEntity(Meeting entity)
    {
        var teachers = entity.MeetingParticipants
            .Select(mp => new TeacherResource(
                mp.TeacherId,
                mp.Teacher?.ProfileName?.FirstName ?? "UnKnown",
                mp.Teacher?.ProfileName?.LastName ?? ""
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