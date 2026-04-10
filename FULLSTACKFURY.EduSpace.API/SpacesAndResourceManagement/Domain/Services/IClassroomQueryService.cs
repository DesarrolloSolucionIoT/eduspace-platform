using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

/// <summary>
///     Represents the classroom query service in the EduSpace API.
/// </summary>
public interface IClassroomQueryService
{
    /// <summary>
    ///     Handles the get classroom by id query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetClassroomByIdQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     The <see cref="Classroom" /> entity.
    /// </returns>
    Task<Classroom?> Handle(GetClassroomByIdQuery query);

    /// <summary>
    ///     Handles the get all classrooms query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetAllClassroomsQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     A collection of all classrooms in the platform.
    /// </returns>
    Task<IEnumerable<Classroom>> Handle(GetAllClassroomsQuery query);

    /// <summary>
    ///     Handles the get all classrooms by teacher id query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetAllClassroomsByTeacherIdQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     A collection of classrooms that belong to the teacher.
    /// </returns>
    Task<IEnumerable<Classroom>> Handle(GetAllClassroomsByTeacherIdQuery query);
}