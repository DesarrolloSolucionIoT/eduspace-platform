namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;

public record SignUpCommand(string Username, string Password, string Role);