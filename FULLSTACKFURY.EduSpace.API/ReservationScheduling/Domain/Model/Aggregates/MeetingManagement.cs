using System.ComponentModel.DataAnnotations.Schema;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Entities;
using TeacherId = FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.ValueObjects.TeacherId;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;

public partial class Meeting
{
    public ICollection<MeetingSession> MeetingParticipants { get; } = new List<MeetingSession>();

    [NotMapped] public TeacherId TeacherId { get; set; }

    public void AddTeacherToMeeting(int teacherId)
    {
        MeetingParticipants.Add(new MeetingSession(teacherId, Id));
    }

    public bool RemoveTeacherFromMeeting(int teacherId)
    {
        var participant = MeetingParticipants.FirstOrDefault(mp => mp.TeacherId == teacherId);
        if (participant == null)
            return false;

        MeetingParticipants.Remove(participant);
        return true;
    }

    public void TeacherIdBuilder(int teacherId)
    {
        TeacherId = new TeacherId(teacherId);
    }
}