namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

/// <summary>
/// Request body for <c>POST /api/v1/authentication/forgot-password</c>.
/// </summary>
public record ForgotPasswordResource(string Email);
