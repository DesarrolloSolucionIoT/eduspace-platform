namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

/// <summary>
/// Request body for <c>POST /api/v1/authentication/reset-password</c>.
/// </summary>
public record ResetPasswordResource(string Token, string NewPassword);
