using System.Net.Http.Headers;
using System.Net.Http.Json;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly HttpClient _http;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _http = httpClientFactory.CreateClient("Resend");
        _http.BaseAddress ??= new Uri("https://api.resend.com/");
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var apiKey = _configuration["RESEND_API_KEY"]
                     ?? throw new InvalidOperationException("RESEND_API_KEY not configured");
        var fromEmail = _configuration["RESEND_FROM"] ?? "onboarding@resend.dev";
        var fromName = _configuration["RESEND_FROM_NAME"] ?? "EduSpace Platform";

        var payload = new
        {
            from = $"{fromName} <{fromEmail}>",
            to = new[] { to },
            subject,
            html = $"<h3>Tu código es: {body}</h3>",
            text = $"Tu código es: {body}"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails") { Content = JsonContent.Create(payload) };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Resend send failed: {Status} {Body}", response.StatusCode, error);
            throw new InvalidOperationException($"Resend failed: {response.StatusCode}");
        }

        _logger.LogInformation("Email enviado a {To} via Resend", to);
    }
}
