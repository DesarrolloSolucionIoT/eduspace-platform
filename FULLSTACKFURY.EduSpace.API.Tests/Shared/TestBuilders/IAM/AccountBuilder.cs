using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;

/// <summary>
/// Fluent builder for <see cref="Account"/> test instances.
/// Instantiates Account directly — no mocks for same-context aggregates.
/// </summary>
public class AccountBuilder
{
    private string _username = "testuser@example.com";
    private string _passwordHash = "hashed_password_123";
    private string _role = "RoleAdmin";

    public AccountBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public AccountBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public AccountBuilder WithRole(string role)
    {
        _role = role;
        return this;
    }

    public AccountBuilder AsAdmin()
    {
        _role = "RoleAdmin";
        return this;
    }

    public AccountBuilder AsTeacher()
    {
        _role = "RoleTeacher";
        return this;
    }

    private bool _activate = false;

    /// <summary>
    /// Calls <see cref="Account.Activate()"/> on the built instance so <see cref="Account.IsActive"/> is true.
    /// </summary>
    public AccountBuilder AsActive()
    {
        _activate = true;
        return this;
    }

    public Account Build()
    {
        var account = new Account(_username, _passwordHash, _role);
        if (_activate) account.Activate();
        return account;
    }
}
