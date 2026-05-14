using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

public record AccountId
{
    public int Id { get; init; }

    // EF Core parameterless ctor
    private AccountId() { }

    public AccountId(int id)
    {
        if (id <= 0)
            throw new InvalidProfileDataException("AccountId must be greater than zero.");
        Id = id;
    }
}
