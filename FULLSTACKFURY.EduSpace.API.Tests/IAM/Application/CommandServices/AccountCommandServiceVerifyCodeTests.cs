using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.CommandServices;

public class AccountCommandServiceVerifyCodeTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IVerificationCodeRepository _verificationCodeRepository = Substitute.For<IVerificationCodeRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IHashingService _hashingService = Substitute.For<IHashingService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IRefreshTokenService _refreshTokenService = Substitute.For<IRefreshTokenService>();
    private readonly ITeacherProfileRepository _teacherProfileRepository = Substitute.For<ITeacherProfileRepository>();
    private readonly IAdminProfileRepository _adminProfileRepository = Substitute.For<IAdminProfileRepository>();
    private readonly IClassroomQueryService _classroomQueryService = Substitute.For<IClassroomQueryService>();
    private readonly IMeetingQueryService _meetingQueryService = Substitute.For<IMeetingQueryService>();
    private readonly ILogger<AccountCommandService> _logger = Substitute.For<ILogger<AccountCommandService>>();

    private AccountCommandService CreateSut() => new(
        _unitOfWork, _accountRepository, _verificationCodeRepository,
        _tokenService, _hashingService, _emailService, _refreshTokenService,
        _teacherProfileRepository, _adminProfileRepository,
        _classroomQueryService, _meetingQueryService, _logger);

    private VerificationCode BuildActiveCode(int accountId, string code = "123456") => new()
    {
        AccountId = accountId,
        Code = code,
        ExpirationDate = DateTime.UtcNow.AddMinutes(10),
        Account = new AccountBuilder().Build()
    };

    // ─── VerifyCode — account not found ─────────────────────────────────────────

    [Fact]
    public async Task Handle_VerifyCode_WhenAccountNotFound_ThrowsAccountNotFoundException()
    {
        // Arrange
        const string username = "ghost@example.com";
        _accountRepository.FindByUsername(username).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new VerifyCodeCommand(username, "123456"));

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
    }

    // ─── VerifyCode — invalid code ───────────────────────────────────────────────

    [Fact]
    public async Task Handle_VerifyCode_WhenCodeIsInvalid_ThrowsInvalidVerificationCodeException()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();

        _accountRepository.FindByUsername(username).Returns(account);
        _verificationCodeRepository
            .FindActiveByAccountIdAndCodeAsync(account.Id, "wrong_code")
            .Returns((VerificationCode?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new VerifyCodeCommand(username, "wrong_code"));

        // Assert
        await act.Should().ThrowAsync<InvalidVerificationCodeException>();
    }

    // ─── VerifyCode — happy path (admin) ─────────────────────────────────────────

    [Fact]
    public async Task Handle_VerifyCode_WhenValidCodeAndAdminRole_ReturnsAccessAndRefreshTokens()
    {
        // Arrange
        const string username = "admin@example.com";
        const string code = "123456";
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();
        var verificationCode = BuildActiveCode(account.Id, code);
        var adminProfile = ProfileTestHelper.CreateAdminProfile(email: "admin@example.com");

        _accountRepository.FindByUsername(username).Returns(account);
        _verificationCodeRepository
            .FindActiveByAccountIdAndCodeAsync(account.Id, code)
            .Returns(verificationCode);
        _tokenService.GenerateToken(account).Returns("access_token_value");
        var (dummyEntity, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService
            .CreateForAccountAsync(account.Id)
            .Returns(("raw_refresh_token", dummyEntity));
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        var (returnedAccount, accessToken, refreshToken, profileId, teacherProfile, returnedAdmin, classrooms, meetings)
            = await sut.Handle(new VerifyCodeCommand(username, code));

        // Assert
        accessToken.Should().Be("access_token_value");
        refreshToken.Should().Be("raw_refresh_token");
        returnedAccount.Should().BeSameAs(account);
    }

    [Fact]
    public async Task Handle_VerifyCode_WhenValidCode_MarksCodeAsUsedAndPersists()
    {
        // Arrange
        const string username = "admin@example.com";
        const string code = "654321";
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();
        var verificationCode = BuildActiveCode(account.Id, code);
        var adminProfile = ProfileTestHelper.CreateAdminProfile();

        _accountRepository.FindByUsername(username).Returns(account);
        _verificationCodeRepository
            .FindActiveByAccountIdAndCodeAsync(account.Id, code)
            .Returns(verificationCode);
        _tokenService.GenerateToken(account).Returns("token");
        var (dummyEntity2, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService
            .CreateForAccountAsync(account.Id)
            .Returns(("refresh", dummyEntity2));
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        await sut.Handle(new VerifyCodeCommand(username, code));

        // Assert
        verificationCode.IsUsed.Should().BeTrue();
        await _unitOfWork.Received().CompleteAsync();
    }

    // ─── VerifyCode — happy path (teacher) ──────────────────────────────────────

    [Fact]
    public async Task Handle_VerifyCode_WhenValidCodeAndTeacherRole_ReturnsTeacherProfile()
    {
        // Arrange
        const string username = "teacher@example.com";
        const string code = "111222";
        var account = new AccountBuilder().WithUsername(username).AsTeacher().Build();
        var verificationCode = BuildActiveCode(account.Id, code);
        var teacherProfile = ProfileTestHelper.CreateTeacherProfile(email: "teacher@example.com");

        _accountRepository.FindByUsername(username).Returns(account);
        _verificationCodeRepository
            .FindActiveByAccountIdAndCodeAsync(account.Id, code)
            .Returns(verificationCode);
        _tokenService.GenerateToken(account).Returns("token");
        var (dummyEntity3, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService
            .CreateForAccountAsync(account.Id)
            .Returns(("refresh", dummyEntity3));
        _teacherProfileRepository.FindByAccountIdAsync(account.Id).Returns(teacherProfile);
        _classroomQueryService
            .Handle(Arg.Any<global::FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries.GetAllClassroomsByTeacherIdQuery>())
            .Returns(Enumerable.Empty<global::FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates.Classroom>());
        _meetingQueryService
            .Handle(Arg.Any<global::FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Queries.GetAllMeetingByTeacherIdQuery>())
            .Returns(Enumerable.Empty<global::FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates.Meeting>());

        var sut = CreateSut();

        // Act
        var (_, _, _, _, returnedTeacher, returnedAdmin, classrooms, meetings)
            = await sut.Handle(new VerifyCodeCommand(username, code));

        // Assert
        returnedTeacher.Should().BeSameAs(teacherProfile);
        returnedAdmin.Should().BeNull();
    }
}
