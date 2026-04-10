using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Transform;

public static class UpdateMeetingCommandFromResourceAssembler
{
    public static UpdateMeetingCommand ToCommandFromResource(int meetingId, UpdateMeetingResource resource)
    {
        return new UpdateMeetingCommand(
            meetingId,
            resource.Title,
            resource.Description,
            resource.Date,
            resource.Start,
            resource.End,
            resource.AdministratorId,
            resource.ClassroomId);
    }
}