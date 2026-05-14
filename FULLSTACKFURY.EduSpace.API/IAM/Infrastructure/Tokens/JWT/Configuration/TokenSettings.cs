namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Configuration;

public class TokenSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "EduSpace";
    public string Audience { get; set; } = "EduSpaceUsers";
    public int AccessTokenLifetimeMinutes { get; set; } = 60;
    public int RefreshTokenLifetimeDays { get; set; } = 14;
}
