using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;

/// <summary>
/// Helpers for creating cross-context Profile objects used by IAM service tests.
/// </summary>
public static class ProfileTestHelper
{
    public static TeacherProfile CreateTeacherProfile(
        string email = "teacher@example.com",
        int accountId = 1,
        int administratorId = 10)
    {
        return new TeacherProfile(
            firstName: "Jane",
            lastName: "Doe",
            email: email,
            dni: "12345678",
            address: "Av. Lima 123",
            phone: "987654321",
            accountId: new AccountId(accountId),
            administratorId: administratorId);
    }

    public static AdminProfile CreateAdminProfile(
        string email = "admin@example.com",
        int accountId = 1)
    {
        return new AdminProfile(
            firstName: "John",
            lastName: "Smith",
            email: email,
            dni: "87654321",
            address: "Av. Lima 456",
            phone: "912345678",
            accountId: new AccountId(accountId));
    }
}
