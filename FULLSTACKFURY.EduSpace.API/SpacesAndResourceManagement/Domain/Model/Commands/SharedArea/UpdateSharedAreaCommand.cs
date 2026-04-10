namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;

public record UpdateSharedAreaCommand(
    int Id,
    string Name,
    int Capacity,
    string Description
);