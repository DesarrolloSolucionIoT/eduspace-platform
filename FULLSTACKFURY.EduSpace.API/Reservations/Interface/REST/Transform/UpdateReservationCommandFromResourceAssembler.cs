using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Interface.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Interface.REST.Transform;

public static class UpdateReservationCommandFromResourceAssembler
{
    public static UpdateReservationCommand ToCommandFromResource(int id, UpdateReservationResource resource)
    {
        return new UpdateReservationCommand(id, resource.Title, resource.Start, resource.End);
    }
}