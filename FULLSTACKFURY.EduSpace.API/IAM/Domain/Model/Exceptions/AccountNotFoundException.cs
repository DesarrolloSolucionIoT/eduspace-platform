namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class AccountNotFoundException(string message = "Account not found.") : Exception(message);
