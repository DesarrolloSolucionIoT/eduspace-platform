namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;

public record RequestAccountActivationCommand(int AccountId, string Email, string FullName);
