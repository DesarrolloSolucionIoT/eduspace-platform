namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public interface IExternalClassroomService
{
    Task<bool> ValidateClassroomIdAsync(int id);
}
