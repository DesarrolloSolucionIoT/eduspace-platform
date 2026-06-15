using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

public class SharedAreaReservation
{
    [Key] public int Id { get; private set; }
    public int SharedAreaId { get; private set; }
    public int TeacherId { get; private set; }
    public DateOnly ReservationDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public String Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public SharedArea? SharedArea { get; private set; }

    private SharedAreaReservation() { Reason = string.Empty; }

    public SharedAreaReservation(int sharedAreaId, int teacherId, DateOnly reservationDate, TimeOnly startTime, TimeOnly endTime, string reason)
    {
        SharedAreaId = sharedAreaId;
        TeacherId = teacherId;
        ReservationDate = reservationDate;
        StartTime = startTime;
        EndTime = endTime;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }
}