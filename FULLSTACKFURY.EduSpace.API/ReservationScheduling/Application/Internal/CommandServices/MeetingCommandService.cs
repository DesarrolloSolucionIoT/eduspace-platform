using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.CommandServices;

// CROSS-CUTTING NOTE: IUnitOfWork.BeginTransactionAsync() is needed for atomic
// conflict-check + save. Until it is added to Shared, we simulate transactionality
// by re-checking conflicts immediately before SaveChanges. When BeginTransactionAsync
// is available, wrap the conflict-check block in:
//   await using var tx = await unitOfWork.BeginTransactionAsync();
//   ... check + save ...
//   await tx.CommitAsync();
public class MeetingCommandService(
    IMeetingRepository meetingRepository,
    IUnitOfWork unitOfWork,
    IExternalProfileService externalProfileService,
    IExternalClassroomService externalClassroomService) : IMeetingCommandService
{
    public async Task<Meeting?> Handle(CreateMeetingCommand command)
    {
        if (!await externalProfileService.ValidateAdminIdExistenceAsync(command.AdministratorId))
            throw new AdministratorNotFoundForMeetingException(command.AdministratorId);

        if (!await externalClassroomService.ValidateClassroomIdAsync(command.ClassroomId))
            throw new ClassroomNotFoundForMeetingException(command.ClassroomId);

        // Meeting ctor enforces schedule invariants (EnsureValidSchedule)
        var meeting = new Meeting(command);

        await meetingRepository.AddAsync(meeting);
        await unitOfWork.CompleteAsync();

        return meeting;
    }

    public async Task Handle(DeleteMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId)
            ?? throw new MeetingNotFoundException(command.MeetingId);

        meetingRepository.Remove(meeting);
        await unitOfWork.CompleteAsync();
    }

    public async Task<Meeting?> Handle(UpdateMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId)
            ?? throw new MeetingNotFoundException(command.MeetingId);

        var dateChanged = command.Date != meeting.Date;
        var timeChanged = command.Start != meeting.StartTime || command.End != meeting.EndTime;

        // Transactional conflict detection: re-fetch schedules inside the same unit of work
        // to minimise the TOCTOU window. A DB-level UNIQUE constraint on
        // (teacher_profile_id, date, start_time, end_time) is the definitive guard —
        // see CROSS_CUTTING_NEEDS in the fix report.
        if ((dateChanged || timeChanged) && meeting.MeetingParticipants.Any())
            foreach (var participant in meeting.MeetingParticipants)
            {
                var teacherMeetings = await meetingRepository.FindAllByTeacherIdAsync(participant.TeacherId);

                var hasConflict = teacherMeetings.Any(existing =>
                    existing.Id != meeting.Id &&
                    existing.Date == command.Date &&
                    command.Start < existing.EndTime &&
                    command.End > existing.StartTime
                );

                if (hasConflict)
                    throw new MeetingConflictException(participant.TeacherId, command.Date);
            }

        meeting.UpdateTitle(command.Title);
        meeting.UpdateDescription(command.Description);
        meeting.UpdateDate(command.Date);
        meeting.UpdateTime(command.Start, command.End);

        meeting.UpdateAdministrator(
            command.AdministratorId,
            id => externalProfileService.ValidateAdminIdExistenceAsync(id).GetAwaiter().GetResult());

        meeting.UpdateClassroom(
            command.ClassroomId,
            id => externalClassroomService.ValidateClassroomIdAsync(id).GetAwaiter().GetResult());

        meetingRepository.Update(meeting);
        await unitOfWork.CompleteAsync();

        return meeting;
    }

    public async Task Handle(AddTeacherToMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId)
            ?? throw new MeetingNotFoundException(command.MeetingId);

        if (!await externalProfileService.ValidateTeacherExistenceAsync(command.TeacherId))
            throw new TeacherNotFoundForMeetingException(command.TeacherId);

        // In-memory duplicate check (aggregate enforces invariant)
        if (meeting.MeetingParticipants.Any(mp => mp.TeacherId == command.TeacherId))
            throw new TeacherAlreadyInMeetingException(command.TeacherId, command.MeetingId);

        // Transactional conflict detection — re-fetch inside the same unit of work.
        var teacherMeetings = await meetingRepository.FindAllByTeacherIdAsync(command.TeacherId);

        var hasConflict = teacherMeetings.Any(existing =>
            existing.Date == meeting.Date &&
            meeting.StartTime < existing.EndTime &&
            meeting.EndTime > existing.StartTime
        );

        if (hasConflict)
            throw new MeetingConflictException(command.TeacherId, meeting.Date);

        meeting.AddTeacherToMeeting(command.TeacherId);
        await unitOfWork.CompleteAsync();
    }

    public async Task Handle(RemoveTeacherFromMeetingCommand command)
    {
        var meeting = await meetingRepository.FindByIdAsync(command.MeetingId)
            ?? throw new MeetingNotFoundException(command.MeetingId);

        if (!await externalProfileService.ValidateTeacherExistenceAsync(command.TeacherId))
            throw new TeacherNotFoundForMeetingException(command.TeacherId);

        var removed = meeting.RemoveTeacherFromMeeting(command.TeacherId);

        if (!removed)
            throw new TeacherNotInMeetingException(command.TeacherId, command.MeetingId);

        await unitOfWork.CompleteAsync();
    }
}
