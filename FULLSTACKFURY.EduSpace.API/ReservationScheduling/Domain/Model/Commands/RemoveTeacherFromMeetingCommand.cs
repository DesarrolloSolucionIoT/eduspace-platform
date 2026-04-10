namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;

public record RemoveTeacherFromMeetingCommand(int TeacherId, int MeetingId);