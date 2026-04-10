namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Application.Internal.OutboundServices;

public interface IExternalProfileService
{
    bool ValidateTeacherIdExistence(int teacherId);
}