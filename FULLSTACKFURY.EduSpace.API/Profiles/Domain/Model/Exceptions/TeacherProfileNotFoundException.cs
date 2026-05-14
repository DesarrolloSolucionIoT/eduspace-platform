namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;

public class TeacherProfileNotFoundException : ProfileNotFoundException
{
    public TeacherProfileNotFoundException(int id)
        : base($"Teacher profile with ID {id} not found.")
    {
    }
}
