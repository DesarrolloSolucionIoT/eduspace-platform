using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Entities;

// NOTE: TeacherProfile navigation was removed — ACL boundary violation (issue #2).
// Only TeacherId (FK) is kept here. Teacher name resolution goes through
// IExternalProfileService.GetTeacherSummaryAsync when needed by the assembler.
// TODO[ACL]: implement GetTeacherSummaryAsync in ExternalProfileService and call it
//            from MeetingResourceFromEntityAssembler — see Fase 5.
public class MeetingSession
{
    public MeetingSession()
    {
        Meeting = default!;
    }

    public MeetingSession(int teacherId, int meetingId)
    {
        TeacherId = teacherId;
        MeetingId = meetingId;
        Meeting = default!;
    }

    public int MeetingId { get; set; }

    public int TeacherId { get; set; }

    public Meeting Meeting { get; set; }
}
