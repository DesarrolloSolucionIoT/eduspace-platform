namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;

public record ResetPasswordCommand(string RawToken, string NewPassword);
