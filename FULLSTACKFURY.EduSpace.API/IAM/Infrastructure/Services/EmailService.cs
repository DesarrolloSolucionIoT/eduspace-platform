using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            // 1. Lee la API Key desde las variables de Railway
            var apiKey = _configuration["SENDGRID_API_KEY"] ??
                         throw new InvalidOperationException("SENDGRID_API_KEY not configured");
            var client = new SendGridClient(apiKey);

            // 2. Lee el email verificado (el "de") desde las variables de Railway
            var fromEmail = _configuration["SMTP_USER"] ??
                            throw new InvalidOperationException("SMTP_USER (verified sender email) not configured");
            var fromName = _configuration["SENDGRID_FROM_NAME"] ?? "Plataforma EduSpace";

            // 3. Crea el mensaje usando SendGrid
            var msg = new SendGridMessage
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = subject,
                PlainTextContent = $"Tu código es: {body}",
                HtmlContent = $"<h3>Tu código es: {body}</h3>"
            };
            msg.AddTo(new EmailAddress(to));

            // 4. Envía el email usando la API de SendGrid (HTTPS)
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email enviado a {To} via SendGrid API", to);
            }
            else
            {
                _logger.LogError("Fallo al enviar email via SendGrid API: {StatusCode} - {Body}", response.StatusCode,
                    await response.Body.ReadAsStringAsync());
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email con SendGrid API.");
            throw;
        }
    }
}