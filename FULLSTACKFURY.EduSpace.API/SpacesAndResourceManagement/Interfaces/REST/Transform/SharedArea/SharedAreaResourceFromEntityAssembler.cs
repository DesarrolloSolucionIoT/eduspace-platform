using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;

/// <summary>
///     Assembler class to transform CreateSharedAreaResource to CreateSharedAreaCommand
/// </summary>
public class SharedAreaResourceFromEntityAssembler
{
    /// <summary>
    ///     Transform shared area to shared area resource
    /// </summary>
    /// <param name="entity">
    ///     The <see cref="SharedArea" /> entity to transform
    /// </param>
    /// <returns>
    ///     The resulting <see cref="SharedAreaResource" /> resource with the values from the entity
    /// </returns>
    public static SharedAreaResource ToResourceFromEntity(Domain.Model.Aggregates.SharedArea entity)
    {
        return new SharedAreaResource(entity.Id, entity.Name, entity.Capacity, entity.Description);
    }
}