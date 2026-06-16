namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;

public record GetAllReservationsBySharedAreaIdQuery(
    int SharedAreaId,
    DateOnly Date
);