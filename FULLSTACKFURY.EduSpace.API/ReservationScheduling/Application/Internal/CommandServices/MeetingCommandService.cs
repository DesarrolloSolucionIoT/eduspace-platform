using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.CommandServices;

public class MeetingCommandService(
    IMeetingRepository meetingRepository,
    IUnitOfWork unitOfWork,
    IRExternalProfileService externalProfileService,
    IExternalClassroomService externalClassroomService) : IMeetingCommandService
{
    public async Task<Meeting?> Handle(CreateMeetingCommand command)
    {
        if (!externalProfileService.ValidateAdminIdExistence(command.AdministratorId))
            throw new ArgumentException("Admin ID does not exist.");

        if (!externalClassroomService.ValidateClassroomId(command.ClassroomId))
            throw new ArgumentException("Classroom does not exist.");

        var meeting = new Meeting(command);

        await meetingRepository.AddAsync(meeting);

        await unitOfWork.CompleteAsync();

        return meeting;
    }

    public async Task Handle(DeleteMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId);
        if (meeting == null) throw new ArgumentException("Meeting not found.");

        meetingRepository.Remove(meeting);

        await unitOfWork.CompleteAsync();
    }

    public async Task<Meeting?> Handle(UpdateMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId);
        if (meeting == null)
            throw new ArgumentException("Meeting not found.");

        var dateChanged = command.Date != meeting.Date;
        var timeChanged = command.Start != meeting.StartTime || command.End != meeting.EndTime;

        if ((dateChanged || timeChanged) && meeting.MeetingParticipants.Any())
            foreach (var participant in meeting.MeetingParticipants)
            {
                var teacherMeetings = await meetingRepository.FindAllByTeacherIdAsync(participant.TeacherId);

                var hasConflict = teacherMeetings.Any(existingMeeting =>
                    existingMeeting.Id != meeting.Id &&
                    existingMeeting.Date == command.Date &&
                    command.Start < existingMeeting.EndTime &&
                    command.End > existingMeeting.StartTime
                );

                if (hasConflict)
                    throw new ArgumentException(
                        $"The updated time conflicts with another meeting for teacher ID {participant.TeacherId} on {command.Date}.");
            }

        meeting.UpdateTitle(command.Title);
        meeting.UpdateDescription(command.Description);
        meeting.UpdateDate(command.Date);
        meeting.UpdateTime(command.Start, command.End);

        meeting.UpdateAdministrator(command.AdministratorId, externalProfileService.ValidateAdminIdExistence);
        meeting.UpdateClassroom(command.ClassroomId, externalClassroomService.ValidateClassroomId);

        meetingRepository.Update(meeting);
        await unitOfWork.CompleteAsync();

        return meeting;
    }

    public async Task Handle(AddTeacherToMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId);

        if (meeting == null)
            throw new ArgumentException("Meeting not found.");
        if (!externalProfileService.ValidateTeacherExistence(command.TeacherId))
            throw new ArgumentException("Teacher does not exist.");

        if (meeting.MeetingParticipants.Any(mp => mp.TeacherId == command.TeacherId))
            throw new ArgumentException("Teacher is already part of this meeting.");

        var teacherMeetings = await meetingRepository.FindAllByTeacherIdAsync(command.TeacherId);

        var hasConflict = teacherMeetings.Any(existingMeeting =>
            existingMeeting.Date == meeting.Date &&
            meeting.StartTime < existingMeeting.EndTime &&
            meeting.EndTime > existingMeeting.StartTime
        );

        if (hasConflict)
            throw new ArgumentException(
                $"The teacher is already scheduled for another meeting at this time on {meeting.Date}.");

        meeting.AddTeacherToMeeting(command.TeacherId);

        await unitOfWork.CompleteAsync();
    }

    public async Task Handle(RemoveTeacherFromMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId);

        if (meeting == null)
            throw new ArgumentException("Meeting not found.");

        if (!externalProfileService.ValidateTeacherExistence(command.TeacherId))
            throw new ArgumentException("Teacher does not exist.");

        var removed = meeting.RemoveTeacherFromMeeting(command.TeacherId);

        if (!removed)
            throw new ArgumentException("Teacher is not associated with this meeting.");

        await unitOfWork.CompleteAsync();
    }
}