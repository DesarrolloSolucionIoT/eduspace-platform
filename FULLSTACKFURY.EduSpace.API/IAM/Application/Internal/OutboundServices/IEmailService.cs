namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}