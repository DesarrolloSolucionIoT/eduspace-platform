using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;

/// <summary>
///     Repository interface for <see cref="Classroom" /> aggregates.
/// </summary>
public interface IClassroomRepository : IBaseRepository<Classroom>
{
    /// <summary>Returns all classrooms assigned to a given teacher.</summary>
    Task<IEnumerable<Classroom>> FindByTeacherIdAsync(int teacherId);

    /// <summary>Returns <c>true</c> if a classroom with the given name already exists.</summary>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>Returns <c>true</c> if a classroom with the given ID exists.</summary>
    Task<bool> ExistsByClassroomIdAsync(int id);
}
