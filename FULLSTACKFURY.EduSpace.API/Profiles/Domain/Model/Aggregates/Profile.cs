using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.CreatedUpdatedDate.Contracts;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;

public class Profile : IEntityWithCreatedUpdatedDate
{
    public Profile(string firstName, string lastName
        , string email, string dni, string address
        , string phone, AccountId accountId)
    {
        ProfileName = new ProfileName(firstName, lastName);
        ProfilePrivateInformation = new ProfilePrivateInformation(email, dni, address, phone);
        AccountId = accountId;
    }

    public Profile()
    {
        ProfileName = new ProfileName();
        ProfilePrivateInformation = new ProfilePrivateInformation();
        AccountId = default!;
    }

    public int Id { get; }
    public ProfileName ProfileName { get; protected set; }
    public ProfilePrivateInformation ProfilePrivateInformation { get; protected set; }

    public AccountId AccountId { get; private set; }

    public string ProfileFullName => ProfileName.FullName;
    public string ProfileEmail => ProfilePrivateInformation.ObtainEmail;
    public string ProfileDni => ProfilePrivateInformation.ObtainDni;

    [Column("CreatedAt")] public DateTimeOffset? CreatedDate { get; set; }
    [Column("UpdatedAt")] public DateTimeOffset? UpdatedDate { get; set; }
}