using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Constants;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.CommandServices;

public class AdminProfileCommandService(
    IAdminProfileRepository adminProfileRepository,
    IUnitOfWork unitOfWork,
    IExternalIamService externalIamService,
    ILogger<AdminProfileCommandService> logger) : IAdminProfileCommandService
{
    public async Task<AdminProfile?> Handle(CreateAdministratorProfileCommand command)
    {
        try
        {
            var accountId = await externalIamService.CreateAccount(command.Email, command.Password, ProfileRoles.Admin);
            var adminProfile = new AdminProfile(command, accountId);

            await adminProfileRepository.AddAsync(adminProfile);
            await unitOfWork.CompleteAsync();

            return adminProfile;
        }
        catch (InvalidProfileDataException)
        {
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unexpected error occurred while creating administrator profile for email {Email}", command.Email);
            return null;
        }
    }

    public async Task<AdminProfile?> Handle(UpdateAdminProfileCommand command)
    {
        var adminProfile = await adminProfileRepository.FindByIdAsync(command.Id);
        if (adminProfile is null) throw new AdminProfileNotFoundException(command.Id);

        adminProfile.Update(command);
        adminProfileRepository.Update(adminProfile);
        await unitOfWork.CompleteAsync();

        return adminProfile;
    }

    public async Task Handle(DeleteAdminProfileCommand command)
    {
        var adminProfile = await adminProfileRepository.FindByIdAsync(command.Id);
        if (adminProfile is null) throw new AdminProfileNotFoundException(command.Id);

        adminProfileRepository.Remove(adminProfile);
        await unitOfWork.CompleteAsync();
    }
}
