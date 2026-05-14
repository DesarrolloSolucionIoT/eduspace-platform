using System.Text.RegularExpressions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

public record ProfilePrivateInformation
{
    private static readonly Regex DniRegex = new(@"^\d{8}$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^9\d{8}$", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    // EF Core parameterless ctor
    private ProfilePrivateInformation()
    {
        Email = string.Empty;
        Dni = string.Empty;
        Address = string.Empty;
        Phone = string.Empty;
    }

    public ProfilePrivateInformation(string email, string dni, string address, string phone)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            throw new InvalidProfileDataException("Invalid email format.");
        if (!DniRegex.IsMatch(dni))
            throw new InvalidProfileDataException("DNI must be exactly 8 digits.");
        if (string.IsNullOrWhiteSpace(address))
            throw new InvalidProfileDataException("Address is required.");
        if (!PhoneRegex.IsMatch(phone))
            throw new InvalidProfileDataException("Phone must be 9 digits starting with 9.");

        Email = email;
        Dni = dni;
        Address = address;
        Phone = phone;
    }

    public string Email { get; init; }
    public string Dni { get; init; }
    public string Address { get; init; }
    public string Phone { get; init; }
}
