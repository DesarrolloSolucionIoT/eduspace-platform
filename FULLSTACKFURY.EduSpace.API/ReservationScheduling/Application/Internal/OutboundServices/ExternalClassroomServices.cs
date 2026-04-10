using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public class ExternalClassroomServices(ISpacesAndResourceManagementFacade spacesFacade) : IExternalClassroomService
{
    public bool ValidateClassroomId(int id)
    {
        return spacesFacade.ValidateClassroomIdExistence(id);
    }
}