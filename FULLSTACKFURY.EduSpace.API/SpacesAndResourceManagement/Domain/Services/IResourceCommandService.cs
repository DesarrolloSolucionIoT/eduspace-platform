using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

/// <summary>
///     Service for handling commands related to resources.
/// </summary>
public interface IResourceCommandService
{
    Task<Resource?> Handle(CreateResourceCommand command);
    Task Handle(DeleteResourceCommand command);
    Task<Resource?> Handle(UpdateResourceCommand command);
}