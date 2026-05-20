using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var apiKey = configuration["SENDGRID_API_KEY"]
                     ?? throw new InvalidOperationException("SENDGRID_API_KEY not configured");
        var fromEmail = configuration["SENDGRID_FROM_EMAIL"] ?? "noreply@eduspace.app";
        var fromName  = configuration["SENDGRID_FROM_NAME"]  ?? "EduSpace";

        var client  = new SendGridClient(apiKey);
        var from    = new EmailAddress(fromEmail, fromName);
        var toAddr  = new EmailAddress(to);
        var msg     = MailHelper.CreateSingleEmail(from, toAddr, subject,
            plainTextContent: $"Tu código es: {body}",
            htmlContent: $"<h3>Tu código es: {body}</h3>");

        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            logger.LogError("SendGrid send failed: {Status} {Body}", response.StatusCode, error);
            throw new InvalidOperationException($"SendGrid failed: {response.StatusCode}");
        }

        logger.LogInformation("Email enviado a {To} via SendGrid", to);
    }

    public async Task SendActivationEmailAsync(string to, string fullName, string rawToken)
    {
        var apiKey = configuration["SENDGRID_API_KEY"]
                     ?? throw new InvalidOperationException("SENDGRID_API_KEY not configured");
        var fromEmail = configuration["SENDGRID_FROM_EMAIL"] ?? "noreply@eduspace.app";
        var fromName  = configuration["SENDGRID_FROM_NAME"]  ?? "EduSpace";

        var frontendBaseUrl = configuration["FRONTEND_BASE_URL"];
        if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            throw new InvalidOperationException(
                "FRONTEND_BASE_URL no está configurado. No se puede generar el enlace de activación.");

        var activationLink = $"{frontendBaseUrl}/activate?token={rawToken}";

        var htmlBody = $"""
            <!DOCTYPE html>
            <html lang="es">
            <body style="font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px;">
              <div style="max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; padding: 32px;">
                <h2 style="color: #1a1a1a;">¡Hola, {fullName}!</h2>
                <p style="color: #444; font-size: 16px;">
                  Gracias por registrarte en <strong>EduSpace</strong>. Para activar tu cuenta y empezar a usarla,
                  hacé clic en el siguiente enlace:
                </p>
                <div style="text-align: center; margin: 32px 0;">
                  <a href="{activationLink}"
                     style="background-color: #4F46E5; color: #ffffff; padding: 14px 28px;
                            text-decoration: none; border-radius: 6px; font-size: 16px; font-weight: bold;">
                    Activar mi cuenta
                  </a>
                </div>
                <p style="color: #666; font-size: 14px;">
                  Si el botón no funciona, copiá y pegá este enlace en tu navegador:<br/>
                  <a href="{activationLink}" style="color: #4F46E5;">{activationLink}</a>
                </p>
                <p style="color: #999; font-size: 13px;">
                  Este enlace es válido por <strong>24 horas</strong>. Si no te registraste en EduSpace, ignorá este correo.
                </p>
              </div>
            </body>
            </html>
            """;

        var client  = new SendGridClient(apiKey);
        var from    = new EmailAddress(fromEmail, fromName);
        var toAddr  = new EmailAddress(to);
        var msg     = MailHelper.CreateSingleEmail(from, toAddr,
            subject: "Activá tu cuenta de EduSpace",
            plainTextContent: $"Hola {fullName},\n\nActivá tu cuenta haciendo clic en: {activationLink}\n\nEste enlace vence en 24 horas.",
            htmlContent: htmlBody);

        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            logger.LogError("SendGrid activation email failed: {Status} {Body}", response.StatusCode, error);
            throw new InvalidOperationException($"SendGrid failed: {response.StatusCode}");
        }

        logger.LogInformation("Activation email enviado a {To} via SendGrid", to);
    }
}
