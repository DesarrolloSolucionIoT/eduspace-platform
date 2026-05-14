namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class AdministratorNotFoundForMeetingException : Exception
{
    public AdministratorNotFoundForMeetingException(int adminId)
        : base($"Administrator with ID {adminId} does not exist.") { }
}
