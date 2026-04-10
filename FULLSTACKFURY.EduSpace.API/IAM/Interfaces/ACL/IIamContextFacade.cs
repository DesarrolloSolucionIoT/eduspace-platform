namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<int> CreateAccount(string username, string password, string role);
}