using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public class ExternalClassroomServices(ISpacesAndResourceManagementFacade spacesFacade) : IExternalClassroomService
{
    public Task<bool> ValidateClassroomIdAsync(int id)
    {
        return Task.FromResult(spacesFacade.ValidateClassroomIdExistence(id));
    }
}
