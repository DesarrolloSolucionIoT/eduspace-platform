namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class AccountNotActivatedException(string message = "Account has not been activated yet.") : Exception(message);
