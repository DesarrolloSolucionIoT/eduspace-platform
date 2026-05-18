using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.Profiles;

/// <summary>
/// Centralises common test-data construction for the Profiles bounded context.
/// Keeps each test file DRY without hiding intent — callers still specify what matters.
/// </summary>
public static class ProfileTestBuilder
{
    // -------------------------------------------------------------------------
    // Value-Object helpers
    // -------------------------------------------------------------------------

    public static AccountId ValidAccountId(int id = 1) => new(id);

    public static ProfileName ValidProfileName(string first = "Ana", string last = "García")
        => new(first, last);

    public static ProfilePrivateInformation ValidPrivateInfo(
        string email = "ana.garcia@edu.pe",
        string dni = "12345678",
        string address = "Av. Lima 123",
        string phone = "987654321")
        => new(email, dni, address, phone);

    // -------------------------------------------------------------------------
    // Command helpers
    // -------------------------------------------------------------------------

    public static CreateTeacherProfileCommand ValidCreateTeacherCommand(
        string username = "teacher_user",
        string email = "teacher@edu.pe") =>
        new(
            FirstName: "Carlos",
            LastName: "Mendoza",
            Email: email,
            Dni: "12345678",
            Address: "Jr. Cusco 456",
            Phone: "912345678",
            AdministratorId: 10,
            Username: username,
            Password: "SecurePass1!");

    public static CreateAdministratorProfileCommand ValidCreateAdminCommand(
        string username = "admin_user",
        string email = "admin@edu.pe") =>
        new(
            FirstName: "Luisa",
            LastName: "Torres",
            Email: email,
            Dni: "87654321",
            Address: "Av. Arequipa 789",
            Phone: "998877665",
            Username: username,
            Password: "SecurePass1!");

    public static UpdateTeacherProfileCommand ValidUpdateTeacherCommand(int id = 1) =>
        new(
            Id: id,
            FirstName: "Carlos",
            LastName: "Mendoza Updated",
            Email: "carlos.updated@edu.pe",
            Dni: "12345678",
            Address: "Nueva Dirección 999",
            Phone: "912345679");

    public static UpdateAdminProfileCommand ValidUpdateAdminCommand(int id = 1) =>
        new(
            Id: id,
            FirstName: "Luisa",
            LastName: "Torres Updated",
            Email: "luisa.updated@edu.pe",
            Dni: "87654321",
            Address: "Nueva Dirección 888",
            Phone: "998877660");

    // -------------------------------------------------------------------------
    // Aggregate helpers
    // -------------------------------------------------------------------------

    public static TeacherProfile ValidTeacherProfile(int administratorId = 10) =>
        new(
            firstName: "Carlos",
            lastName: "Mendoza",
            email: "teacher@edu.pe",
            dni: "12345678",
            address: "Jr. Cusco 456",
            phone: "912345678",
            accountId: ValidAccountId(),
            administratorId: administratorId);

    public static AdminProfile ValidAdminProfile() =>
        new(
            firstName: "Luisa",
            lastName: "Torres",
            email: "admin@edu.pe",
            dni: "87654321",
            address: "Av. Arequipa 789",
            phone: "998877665",
            accountId: ValidAccountId(2));
}
