namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

/// <summary>
/// Request body for <c>POST /api/v1/authentication/activate</c>.
/// </summary>
public record ActivateAccountResource(string Token);
