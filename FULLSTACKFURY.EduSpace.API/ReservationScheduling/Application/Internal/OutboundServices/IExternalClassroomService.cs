namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public interface IExternalClassroomService
{
    bool ValidateClassroomId(int id);
}