namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Commands;

public record CreateReservationCommand(string Title, DateTime Start, DateTime End, int AreaId, int TeacherId);