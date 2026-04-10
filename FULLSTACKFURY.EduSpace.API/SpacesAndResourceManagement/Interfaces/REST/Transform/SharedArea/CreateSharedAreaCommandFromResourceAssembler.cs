using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;

/// <summary>
///     Assembler class to transform CreateSharedAreaResource to CreateSharedAreaCommand
/// </summary>
public class CreateSharedAreaCommandFromResourceAssembler
{
    /// <summary>
    ///     Transform CreateSharedAreaResource to CreateSharedAreaCommand
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="CreateSharedAreaResource" /> resource to transform
    /// </param>
    /// <returns>
    ///     The resulting <see cref="CreateSharedAreaCommand" /> command with the values from the resource
    /// </returns>
    public static CreateSharedAreaCommand ToCommandFromResource(CreateSharedAreaResource resource)
    {
        return new CreateSharedAreaCommand(resource.Name, resource.Capacity, resource.Description);
    }
}