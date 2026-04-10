namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;

public record AddTeacherToMeetingCommand(int TeacherId, int MeetingId);