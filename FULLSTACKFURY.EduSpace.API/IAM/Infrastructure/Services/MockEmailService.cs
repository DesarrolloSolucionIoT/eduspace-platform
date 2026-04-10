using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Services;

public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("MOCK EMAIL SERVICE - Development Mode");
        _logger.LogInformation("========================================");
        _logger.LogInformation("To: {To}", to);
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("Body: {Body}", body);
        _logger.LogInformation("========================================");
        _logger.LogInformation("Email not sent (using mock service)");

        return Task.CompletedTask;
    }
}