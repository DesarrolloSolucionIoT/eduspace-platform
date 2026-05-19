using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Services;

public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;
    private readonly IConfiguration _configuration;

    public MockEmailService(ILogger<MockEmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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

    public Task SendActivationEmailAsync(string to, string fullName, string rawToken)
    {
        var frontendBaseUrl = _configuration["FRONTEND_BASE_URL"] ?? "http://localhost:5173";
        var activationLink = $"{frontendBaseUrl}/activate?token={rawToken}";

        _logger.LogInformation("========================================");
        _logger.LogInformation("MOCK ACTIVATION EMAIL - Development Mode");
        _logger.LogInformation("========================================");
        _logger.LogInformation("To: {To}", to);
        _logger.LogInformation("FullName: {FullName}", fullName);
        _logger.LogInformation("Activation link (deliver manually): {Link}", activationLink);
        _logger.LogInformation("========================================");

        return Task.CompletedTask;
    }
}
