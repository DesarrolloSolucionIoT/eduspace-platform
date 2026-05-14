namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class InvalidVerificationCodeException(string message = "Invalid or expired verification code.") : Exception(message);
