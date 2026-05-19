using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;

public interface IExternalIamService
{
    Task<AccountId> CreateAccount(string username, string password, string role);

    /// <summary>
    /// Activates a teacher account immediately after creation — no email sent (REQ-018).
    /// </summary>
    Task ActivateAccountAsync(int accountId);

    /// <summary>
    /// Sends an activation email to an admin after account creation (REQ-017).
    /// Best-effort: callers must handle failures gracefully.
    /// </summary>
    Task RequestActivationEmailAsync(int accountId, string email, string fullName);

    /// <summary>
    /// Removes the IAM account linked to a profile that was just deleted, so the
    /// username/email can be reused. Best-effort: failures must not surface as 5xx.
    /// </summary>
    Task DeleteAccountAsync(int accountId);
}