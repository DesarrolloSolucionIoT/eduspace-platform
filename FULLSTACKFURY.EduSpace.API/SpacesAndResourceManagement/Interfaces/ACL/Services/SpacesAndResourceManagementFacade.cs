using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL.Services;

public class SpacesAndResourceManagementFacade(IClassroomRepository classroomRepository)
    : ISpacesAndResourceManagementFacade
{
    public bool ValidateClassroomIdExistence(int classroomId)
    {
        return classroomRepository.ExistsByClassroomId(classroomId);
    }
}