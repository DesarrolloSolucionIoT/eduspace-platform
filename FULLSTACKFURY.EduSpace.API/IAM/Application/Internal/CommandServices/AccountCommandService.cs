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
    IActivationTokenRepository activationTokenRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IEmailService emailService,
    IRefreshTokenService refreshTokenService,
    ITeacherProfileRepository teacherProfileRepository,
    IAdminProfileRepository adminProfileRepository,
    IClassroomQueryService classroomQueryService,
    IMeetingQueryService meetingQueryService,
    ILogger<AccountCommandService> logger,
    ILoggerFactory loggerFactory)
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

    /// <summary>
    /// Validates credentials + activation status, then returns the JWT bundle directly.
    /// No 6-digit code. No email. Password check → IsActive check → JWT (REQ-006, REQ-007).
    /// </summary>
    public async Task<(Account account, string accessToken, string refreshToken, int? profileId,
        TeacherProfile? teacherProfile, AdminProfile? adminProfile,
        IEnumerable<Classroom>? classrooms, IEnumerable<Meeting>? meetings)>
        Handle(SignInCommand command)
    {
        var account = await accountRepository.FindByUsername(command.Username);
        if (account is null && command.Username.Contains('@'))
        {
            var accountId = await teacherProfileRepository.FindAccountIdByEmailAsync(command.Username)
                            ?? await adminProfileRepository.FindAccountIdByEmailAsync(command.Username);
            if (accountId is not null)
                account = await accountRepository.FindByIdAsync(accountId.Value);
        }

        if (account is null || !hashingService.VerifyPassword(command.Password, account.PasswordHash))
        {
            logger.LogWarning("Failed sign-in attempt for identifier {Identifier}", command.Username);
            throw new InvalidCredentialsException();
        }

        if (!account.IsActive)
        {
            logger.LogWarning("Sign-in blocked — account {AccountId} is not activated", account.Id);
            throw new AccountNotActivatedException();
        }

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

    /// <summary>
    /// Delegates to <see cref="ActivateAccountCommandHandler"/> (REQ-009).
    /// </summary>
    public async Task Handle(ActivateAccountCommand command)
    {
        var handler = new ActivateAccountCommandHandler(
            activationTokenRepository, accountRepository, unitOfWork,
            loggerFactory.CreateLogger<ActivateAccountCommandHandler>());
        await handler.Handle(command);
    }

    /// <summary>
    /// Delegates to <see cref="RequestAccountActivationCommandHandler"/> (REQ-010).
    /// </summary>
    public async Task Handle(RequestAccountActivationCommand command)
    {
        var handler = new RequestAccountActivationCommandHandler(
            activationTokenRepository, emailService, unitOfWork,
            loggerFactory.CreateLogger<RequestAccountActivationCommandHandler>());
        await handler.Handle(command);
    }

    /// <summary>
    /// Delegates to <see cref="RequestPasswordResetCommandHandler"/>.
    /// </summary>
    public async Task Handle(RequestPasswordResetCommand command)
    {
        var handler = new RequestPasswordResetCommandHandler(
            passwordResetTokenRepository, teacherProfileRepository, adminProfileRepository,
            emailService, unitOfWork,
            loggerFactory.CreateLogger<RequestPasswordResetCommandHandler>());
        await handler.Handle(command);
    }

    /// <summary>
    /// Delegates to <see cref="ResetPasswordCommandHandler"/>.
    /// </summary>
    public async Task Handle(ResetPasswordCommand command)
    {
        var handler = new ResetPasswordCommandHandler(
            passwordResetTokenRepository, accountRepository, hashingService, unitOfWork,
            loggerFactory.CreateLogger<ResetPasswordCommandHandler>());
        await handler.Handle(command);
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
