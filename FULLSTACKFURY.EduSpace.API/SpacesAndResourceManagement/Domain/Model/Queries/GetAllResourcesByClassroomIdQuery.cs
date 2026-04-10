namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;

/// <summary>
///     Represents a query to get all resources by classroom id in the EduSpace API.
/// </summary>
/// <param name="ClassroomId">
///     The id of the classroom to get all resources from
/// </param>
public record GetAllResourcesByClassroomIdQuery(int ClassroomId);