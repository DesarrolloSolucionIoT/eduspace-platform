using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;

public class ReserveSharedAreaCommandFromResourceAssembler
{
    public static ReserveSharedAreaCommand ToCommandFromResource(int sharedAreaId,
    ReserveSharedAreaResource resource)
    {
        return new ReserveSharedAreaCommand(
            sharedAreaId,
            resource.TeacherId,
            resource.ReservationDate,
            resource.StartTime,
            resource.EndTime,
            resource.Reason
            );
        
    }
}