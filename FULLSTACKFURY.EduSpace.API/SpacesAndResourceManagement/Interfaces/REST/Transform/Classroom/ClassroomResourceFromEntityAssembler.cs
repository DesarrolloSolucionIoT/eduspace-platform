using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Classroom;

/// <summary>
///     Assembler class to transform Classroom to ClassroomResource
/// </summary>
public class ClassroomResourceFromEntityAssembler
{
    /// <summary>
    ///     Transform Classroom to ClassroomResource
    /// </summary>
    /// <param name="entity">
    ///     The <see cref="Classroom" /> entity to transform
    /// </param>
    /// <returns>
    ///     The resulting <see cref="ClassroomResource" /> resource with the values from the entity
    /// </returns>
    public static ClassroomResource ToResourceFromEntity(Domain.Model.Aggregates.Classroom entity)
    {
        return new ClassroomResource(entity.Id, entity.Name, entity.Description, entity.TeacherId.TeacherIdentifier);
    }
}