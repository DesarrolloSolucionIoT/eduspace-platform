namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

public record ProfilePrivateInformation
{
    public ProfilePrivateInformation() : this(string.Empty, string.Empty
        , string.Empty, string.Empty)
    {
    }

    public ProfilePrivateInformation(string email, string dni
        , string address, string phone)
    {
        Email = email;
        Dni = dni;
        Address = address;
        Phone = phone;
    }

    public string Email { get; init; }
    public string Dni { get; init; }
    public string Address { get; init; }
    public string Phone { get; init; }
    public string ObtainEmail => $"{Email}";
    public string ObtainDni => $"{Dni}";
}