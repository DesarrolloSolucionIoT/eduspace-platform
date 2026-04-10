namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;

public record VerifyCodeCommand(string Username, string Code);