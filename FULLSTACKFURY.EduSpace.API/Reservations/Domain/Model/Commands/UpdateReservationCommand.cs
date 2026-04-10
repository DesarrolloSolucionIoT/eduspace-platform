namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Commands;

public record UpdateReservationCommand(int Id, string Title, DateTime Start, DateTime End);