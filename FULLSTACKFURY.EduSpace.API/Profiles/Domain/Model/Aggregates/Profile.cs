using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.CreatedUpdatedDate.Contracts;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;

public class Profile : IEntityWithCreatedUpdatedDate
{
    // EF Core ctor — not for handler use
    protected Profile()
    {
        ProfileName = null!;
        ProfilePrivateInformation = null!;
        AccountId = null!;
    }

    public Profile(string firstName, string lastName
        , string email, string dni, string address
        , string phone, AccountId accountId)
    {
        ProfileName = new ProfileName(firstName, lastName);
        ProfilePrivateInformation = new ProfilePrivateInformation(email, dni, address, phone);
        AccountId = accountId;
    }

    public int Id { get; }
    public ProfileName ProfileName { get; protected set; }
    public ProfilePrivateInformation ProfilePrivateInformation { get; protected set; }
    public AccountId AccountId { get; private set; }

    public string ProfileFullName => ProfileName.FullName;
    public string ProfileEmail => ProfilePrivateInformation.Email;
    public string ProfileDni => ProfilePrivateInformation.Dni;

    [Column("CreatedAt")] public DateTimeOffset? CreatedDate { get; set; }
    [Column("UpdatedAt")] public DateTimeOffset? UpdatedDate { get; set; }

    public void UpdateProfileName(string firstName, string lastName)
    {
        ProfileName = new ProfileName(firstName, lastName);
    }

    public void UpdatePrivateInformation(string email, string dni, string address, string phone)
    {
        ProfilePrivateInformation = new ProfilePrivateInformation(email, dni, address, phone);
    }
}
