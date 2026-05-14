namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL;

public interface ISpacesAndResourceManagementFacade
{
    bool ValidateClassroomIdExistence(int classroomId);
    bool ValidateResourceIdExistence(int resourceId);
}