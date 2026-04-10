using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.CommandServices;

public class TeacherProfileCommandService(
    ITeacherProfileRepository teacherProfileRepository,
    IUnitOfWork unitOfWork,
    IExternalIamService externalIamService)
    : ITeacherProfileCommandService
{
    public async Task<TeacherProfile?> Handle(CreateTeacherProfileCommand command)
    {
        try
        {
            var accountId = externalIamService.CreateAccount(command.Username, command.Password, "RoleTeacher");
            var teacherProfile = new TeacherProfile(command, accountId.Result);

            await teacherProfileRepository.AddAsync(teacherProfile);
            await unitOfWork.CompleteAsync();

            return teacherProfile;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while creating the profile {e.Message}");
            return null;
        }
    }

    public async Task<TeacherProfile?> Handle(UpdateTeacherProfileCommand command)
    {
        var teacherProfile = await teacherProfileRepository.FindByIdAsync(command.Id);
        if (teacherProfile == null) throw new ArgumentException($"Teacher profile with ID {command.Id} not found.");

        teacherProfile.Update(command);
        teacherProfileRepository.Update(teacherProfile);
        await unitOfWork.CompleteAsync();

        return teacherProfile;
    }

    public async Task Handle(DeleteTeacherProfileCommand command)
    {
        var teacherProfile = await teacherProfileRepository.FindByIdAsync(command.Id);
        if (teacherProfile == null) throw new ArgumentException($"Teacher profile with ID {command.Id} not found.");

        teacherProfileRepository.Remove(teacherProfile);
        await unitOfWork.CompleteAsync();
    }
}