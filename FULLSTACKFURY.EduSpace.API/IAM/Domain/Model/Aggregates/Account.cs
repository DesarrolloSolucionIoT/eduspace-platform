using System.Text.Json.Serialization;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

public class Account
{
    public Account(string username, string passwordHash, string role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = Enum.Parse<ERoles>(role);
    }

    // Parameterless ctor for EF Core
    private Account()
    {
        Username = string.Empty;
        PasswordHash = string.Empty;
    }

    public int Id { get; }
    public string Username { get; private set; }
    [JsonIgnore] public string PasswordHash { get; private set; }
    public ERoles Role { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Activates the account. Idempotent — safe to call if already active.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateUsername(string username)
    {
        Username = username;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public string GetRole()
    {
        return Role.ToString();
    }
}
