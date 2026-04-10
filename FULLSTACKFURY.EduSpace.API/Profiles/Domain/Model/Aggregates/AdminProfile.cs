using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;

public class AdminProfile : Profile
{
    public AdminProfile(string firstName, string lastName, string email, string dni, string address, string phone,
        AccountId accountId)
        : base(firstName, lastName, email, dni, address, phone, accountId)
    {
    }

    public AdminProfile(CreateAdministratorProfileCommand command, AccountId accountId)
        : base(command.FirstName, command.LastName, command.Email, command.Dni
            , command.Address, command.Phone, accountId)
    {
    }

    public AdminProfile()
    {
    }

    public AdminProfile Update(UpdateAdminProfileCommand command)
    {
        ProfileName = new ProfileName(command.FirstName, command.LastName);
        ProfilePrivateInformation =
            new ProfilePrivateInformation(command.Email, command.Dni, command.Address, command.Phone);
        return this;
    }
}