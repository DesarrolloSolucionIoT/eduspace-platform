namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<int> CreateAccount(string username, string password, string role);

    /// <summary>
    /// Activates an account silently — no email sent. Used for teacher accounts
    /// auto-activated by the admin (REQ-018, REQ-016).
    /// </summary>
    Task ActivateAccountAsync(int accountId);

    /// <summary>
    /// Generates an activation token and sends an email to the user.
    /// Used for admin accounts that require email verification (REQ-017, REQ-016).
    /// </summary>
    Task RequestActivationEmailAsync(int accountId, string email, string fullName);
}
