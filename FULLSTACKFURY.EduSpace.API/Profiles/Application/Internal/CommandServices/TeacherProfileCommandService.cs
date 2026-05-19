using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Constants;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.CommandServices;

public class TeacherProfileCommandService(
    ITeacherProfileRepository teacherProfileRepository,
    IUnitOfWork unitOfWork,
    IExternalIamService externalIamService,
    ILogger<TeacherProfileCommandService> logger)
    : ITeacherProfileCommandService
{
    public async Task<TeacherProfile?> Handle(CreateTeacherProfileCommand command)
    {
        try
        {
            var accountId = await externalIamService.CreateAccount(command.Username, command.Password, ProfileRoles.Teacher);

            // Teachers are auto-activated immediately — no email required (REQ-008 / Design Decision 3).
            await externalIamService.ActivateAccountAsync(accountId.Id);

            var teacherProfile = new TeacherProfile(command, accountId);

            await teacherProfileRepository.AddAsync(teacherProfile);
            await unitOfWork.CompleteAsync();

            return teacherProfile;
        }
        catch (InvalidProfileDataException)
        {
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unexpected error occurred while creating teacher profile for username {Username}", command.Username);
            return null;
        }
    }

    public async Task<TeacherProfile?> Handle(UpdateTeacherProfileCommand command)
    {
        var teacherProfile = await teacherProfileRepository.FindByIdAsync(command.Id);
        if (teacherProfile is null) throw new TeacherProfileNotFoundException(command.Id);

        teacherProfile.Update(command);
        teacherProfileRepository.Update(teacherProfile);
        await unitOfWork.CompleteAsync();

        return teacherProfile;
    }

    public async Task Handle(DeleteTeacherProfileCommand command)
    {
        var teacherProfile = await teacherProfileRepository.FindByIdAsync(command.Id);
        if (teacherProfile is null) throw new TeacherProfileNotFoundException(command.Id);

        var linkedAccountId = teacherProfile.AccountId.Id;

        teacherProfileRepository.Remove(teacherProfile);
        await unitOfWork.CompleteAsync();

        // Cascade-delete the IAM account so the username/email can be reused.
        // Best-effort: mirrors AdminProfileCommandService delete semantics.
        try
        {
            await externalIamService.DeleteAccountAsync(linkedAccountId);
        }
        catch (Exception iamEx)
        {
            logger.LogError(iamEx,
                "Orphan IAM account left behind: teacher profile {ProfileId} was deleted but " +
                "DeleteAccountAsync failed for account {AccountId}. Clean up manually.",
                command.Id, linkedAccountId);
        }
    }
}
