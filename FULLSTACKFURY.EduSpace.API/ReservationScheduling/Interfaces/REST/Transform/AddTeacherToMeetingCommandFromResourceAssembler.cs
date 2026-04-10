using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Transform;

public static class AddTeacherToMeetingCommandFromResourceAssembler
{
    public static AddTeacherToMeetingCommand ToCommandFromResource(AddTeacherToMeetingResource resource)
    {
        return new AddTeacherToMeetingCommand(resource.TeacherId, resource.MeetingId);
    }
}