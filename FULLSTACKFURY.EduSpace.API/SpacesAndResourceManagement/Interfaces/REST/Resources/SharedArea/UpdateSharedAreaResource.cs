namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

public record UpdateSharedAreaResource(
    int Id,
    string Name,
    int Capacity,
    string Description
);