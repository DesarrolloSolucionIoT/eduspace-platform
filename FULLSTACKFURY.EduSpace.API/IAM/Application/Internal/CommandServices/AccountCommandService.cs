using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;

public class AccountCommandService(
    IUnitOfWork unitOfWork,
    IAccountRepository accountRepository,
    IVerificationCodeRepository verificationCodeRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IEmailService emailService,
    IRefreshTokenService refreshTokenService,
    ITeacherProfileRepository teacherProfileRepository,
    IAdminProfileRepository adminProfileRepository,
    IClassroomQueryService classroomQueryService,
    IMeetingQueryService meetingQueryService,
    ILogger<AccountCommandService> logger)
    : IAccountCommandService
{
    public async Task Handle(SignUpCommand command)
    {
        if (accountRepository.ExistsByUsername(command.Username))
            throw new InvalidCredentialsException($"Username {command.Username} is already taken.");

        var hashedPassword = hashingService.HashPassword(command.Password);
        var account = new Account(command.Username, hashedPassword, command.Role);

        try
        {
            await accountRepository.AddAsync(account);
            await unitOfWork.CompleteAsync();
            logger.LogDebug("Account created for username {Username}", command.Username);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating account for username {Username}", command.Username);
            throw new InvalidOperationException($"An error occurred while creating the account: {e.Message}", e);
        }
    }

    public async Task Handle(SignInCommand command)
    {
        var account = await accountRepository.FindByUsername(command.Username);
        if (account is null || !hashingService.VerifyPassword(command.Password, account.PasswordHash))
        {
            logger.LogWarning("Failed sign-in attempt for username {Username}", command.Username);
            throw new InvalidCredentialsException();
        }

        var code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        var verificationCode = new VerificationCode
        {
            AccountId = account.Id,
            Code = code,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        await verificationCodeRepository.AddAsync(verificationCode);
        await unitOfWork.CompleteAsync();

        // Resolve user email via Profiles ACL (N+1 fixed — uses targeted lookup)
        string? userEmail = null;
        var teacher = await teacherProfileRepository.FindByAccountIdAsync(account.Id);
        if (teacher != null)
        {
            userEmail = teacher.ProfilePrivateInformation.Email;
        }
        else
        {
            var admin = await adminProfileRepository.FindByAccountIdAsync(account.Id);
            if (admin != null) userEmail = admin.ProfilePrivateInformation.Email;
        }

        if (string.IsNullOrEmpty(userEmail))
        {
            logger.LogWarning("No profile email found for account {AccountId}", account.Id);
            throw new AccountNotFoundException("User profile email not found. Please complete your profile setup.");
        }

        await emailService.SendEmailAsync(
            userEmail,
            "Tu código de verificación de EduSpace",
            $"Tu código es: {code}");

        logger.LogDebug("Verification code sent to account {AccountId}", account.Id);
    }

    public async Task<(Account account, string accessToken, string refreshToken, int? profileId,
        TeacherProfile? teacherProfile, AdminProfile? adminProfile,
        IEnumerable<Classroom>? classrooms, IEnumerable<Meeting>? meetings)>
        Handle(VerifyCodeCommand command)
    {
        var account = await accountRepository.FindByUsername(command.Username);
        if (account is null)
        {
            logger.LogWarning("Verify code attempted for unknown username {Username}", command.Username);
            throw new AccountNotFoundException();
        }

        var verificationCode = await verificationCodeRepository
            .FindActiveByAccountIdAndCodeAsync(account.Id, command.Code);

        if (verificationCode is null)
        {
            logger.LogWarning("Invalid or expired verification code for account {AccountId}", account.Id);
            throw new InvalidVerificationCodeException();
        }

        verificationCode.MarkAsUsed();
        await unitOfWork.CompleteAsync();

        var accessToken = tokenService.GenerateToken(account);
        var (rawRefreshToken, _) = await refreshTokenService.CreateForAccountAsync(account.Id);

        int? profileId = null;
        TeacherProfile? teacherProfile = null;
        AdminProfile? adminProfile = null;
        IEnumerable<Classroom>? classrooms = null;
        IEnumerable<Meeting>? meetings = null;

        if (account.GetRole() == "RoleTeacher")
        {
            teacherProfile = await teacherProfileRepository.FindByAccountIdAsync(account.Id);
            profileId = teacherProfile?.Id;

            if (teacherProfile != null)
            {
                classrooms = await classroomQueryService.Handle(
                    new GetAllClassroomsByTeacherIdQuery(teacherProfile.Id));
                meetings = await meetingQueryService.Handle(
                    new GetAllMeetingByTeacherIdQuery(teacherProfile.Id));
            }
        }
        else if (account.GetRole() == "RoleAdmin")
        {
            adminProfile = await adminProfileRepository.FindByAccountIdAsync(account.Id);
            profileId = adminProfile?.Id;
        }

        logger.LogDebug("Account {AccountId} successfully authenticated", account.Id);
        return (account, accessToken, rawRefreshToken, profileId, teacherProfile, adminProfile, classrooms, meetings);
    }

    public async Task<(string newAccessToken, string newRefreshToken)> Handle(RefreshAccessTokenCommand command)
    {
        var (newRaw, _, oldEntity) = await refreshTokenService.RotateAsync(command.RefreshToken);

        var account = await accountRepository.FindByIdAsync(oldEntity.AccountId);
        if (account is null)
        {
            logger.LogWarning("Account {AccountId} not found during token refresh", oldEntity.AccountId);
            throw new AccountNotFoundException();
        }

        var newAccessToken = tokenService.GenerateToken(account);
        logger.LogDebug("Access token refreshed for account {AccountId}", account.Id);
        return (newAccessToken, newRaw);
    }

    public async Task Handle(LogoutCommand command)
    {
        await refreshTokenService.RevokeAsync(command.RefreshToken);
        logger.LogDebug("Logout: refresh token revoked");
    }
}
