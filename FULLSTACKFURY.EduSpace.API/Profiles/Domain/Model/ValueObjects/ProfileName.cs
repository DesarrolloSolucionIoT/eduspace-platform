using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

public record ProfileName
{
    // EF Core parameterless ctor
    private ProfileName()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public ProfileName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new InvalidProfileDataException("First name is required.");
        if (firstName.Length > 100)
            throw new InvalidProfileDataException("First name must not exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new InvalidProfileDataException("Last name is required.");
        if (lastName.Length > 100)
            throw new InvalidProfileDataException("Last name must not exceed 100 characters.");

        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }

    public string FullName => $"{FirstName} {LastName}";
}
