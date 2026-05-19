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
            var accountId = await externalIamService.CreateAccount(command.Username, command.Password, ProfileRoles.Admin);
            var adminProfile = new AdminProfile(command, accountId);

            await adminProfileRepository.AddAsync(adminProfile);
            await unitOfWork.CompleteAsync();

            // Best-effort activation email — account already persisted; a delivery failure
            // must not roll back the creation (REQ-021 / Design Decision 4 academic fallback).
            try
            {
                await externalIamService.RequestActivationEmailAsync(
                    accountId.Id,
                    command.Email,
                    $"{command.FirstName} {command.LastName}");
            }
            catch (Exception emailEx)
            {
                logger.LogError(emailEx,
                    "Activation email could not be sent for admin account {AccountId} ({Email}). " +
                    "The account was created successfully — assist the user manually if needed.",
                    accountId.Id, command.Email);
            }

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

        var linkedAccountId = adminProfile.AccountId.Id;

        adminProfileRepository.Remove(adminProfile);
        await unitOfWork.CompleteAsync();

        // Cascade-delete the IAM account so the username/email can be reused.
        // Best-effort: a failure here leaves an orphan account but the profile
        // is already gone — must not 5xx (Design Decision 4, academic fallback).
        try
        {
            await externalIamService.DeleteAccountAsync(linkedAccountId);
        }
        catch (Exception iamEx)
        {
            logger.LogError(iamEx,
                "Orphan IAM account left behind: profile {ProfileId} was deleted but " +
                "DeleteAccountAsync failed for account {AccountId}. Clean up manually.",
                command.Id, linkedAccountId);
        }
    }
}
