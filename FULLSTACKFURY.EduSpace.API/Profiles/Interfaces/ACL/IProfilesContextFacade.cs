namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;

public interface IProfilesContextFacade
{
    bool ValidateTeacherProfileIdExistence(int id);
    bool ValidateAdminProfileIdExistence(int id);
}