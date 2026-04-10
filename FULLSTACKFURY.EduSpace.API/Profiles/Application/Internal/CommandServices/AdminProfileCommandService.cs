using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.CommandServices;

public class AdminProfileCommandService(
    IAdminProfileRepository adminProfileRepository,
    IUnitOfWork unitOfWork,
    IExternalIamService externalIamService) : IAdminProfileCommandService
{
    public async Task<AdminProfile?> Handle(CreateAdministratorProfileCommand command)
    {
        try
        {
            var accountId = externalIamService.CreateAccount(command.Email, command.Password, "RoleAdmin");
            Console.WriteLine("The result of the account creation is: " + accountId.Id);
            var adminProfile = new AdminProfile(command, accountId.Result);

            await adminProfileRepository.AddAsync(adminProfile);

            await unitOfWork.CompleteAsync();
            return adminProfile;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while creating the profile: {e.Message}");
            return null;
        }
    }

    public async Task<AdminProfile?> Handle(UpdateAdminProfileCommand command)
    {
        var adminProfile = await adminProfileRepository.FindByIdAsync(command.Id);
        if (adminProfile == null) throw new ArgumentException($"Admin profile with ID {command.Id} not found.");

        adminProfile.Update(command);
        adminProfileRepository.Update(adminProfile);
        await unitOfWork.CompleteAsync();

        return adminProfile;
    }

    public async Task Handle(DeleteAdminProfileCommand command)
    {
        var adminProfile = await adminProfileRepository.FindByIdAsync(command.Id);
        if (adminProfile == null) throw new ArgumentException($"Admin profile with ID {command.Id} not found.");

        adminProfileRepository.Remove(adminProfile);
        await unitOfWork.CompleteAsync();
    }
}