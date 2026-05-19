namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);

    /// <summary>
    /// Sends the account activation email containing the link
    /// {FRONTEND_BASE_URL}/activate?token={rawToken} in Spanish (Rioplatense voseo).
    /// Reads FRONTEND_BASE_URL from configuration — throws <see cref="InvalidOperationException"/>
    /// if the env var is absent (REQ-021).
    /// </summary>
    Task SendActivationEmailAsync(string to, string fullName, string rawToken);
}
