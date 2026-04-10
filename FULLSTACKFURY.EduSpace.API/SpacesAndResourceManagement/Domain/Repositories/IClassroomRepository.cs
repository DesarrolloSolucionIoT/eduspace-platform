using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;

/// <summary>
///     Represents the classroom repository in the EduSpace Platform
/// </summary>
public interface IClassroomRepository : IBaseRepository<Classroom>
{
    /// <summary>
    ///     Finds a classroom by teacher id asynchronously
    /// </summary>
    /// <param name="teacherId">
    ///     The id of the teacher  to find classrooms by.
    /// </param>
    /// <returns>
    ///     A collection of classrooms that belong to the teacher.
    /// </returns>
    Task<IEnumerable<Classroom>> FindByTeacherIdAsync(int teacherId);

    /// <summary>
    ///     Verify if a classroom with specified title exists.
    /// </summary>
    /// <param name="name">
    ///     The title of the classroom to verify.
    /// </param>
    /// <returns>
    ///     True if the classroom exists, otherwise false.
    /// </returns>
    Task<bool> ExistsByNameAsync(string name);

    bool ExistsByClassroomId(int id);
}