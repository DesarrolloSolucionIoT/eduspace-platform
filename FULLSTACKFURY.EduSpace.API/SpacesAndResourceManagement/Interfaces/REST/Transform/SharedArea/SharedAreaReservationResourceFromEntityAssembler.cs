using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;

public class SharedAreaReservationResourceFromEntityAssembler
{
    public static SharedAreaReservationResource ToResourceFromEntity(SharedAreaReservation entity)
    {
        return new SharedAreaReservationResource(
            entity.Id,
            entity.SharedAreaId,
            entity.SharedArea?.Name ?? string.Empty,
            entity.TeacherId,
            entity.ReservationDate,
            entity.StartTime,
            entity.EndTime,
            entity.Reason,
            entity.CreatedAt
            
        );
    }
}