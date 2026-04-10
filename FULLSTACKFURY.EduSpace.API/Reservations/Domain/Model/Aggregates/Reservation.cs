using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Aggregates;

public class Reservation
{
    public Reservation()
    {
        ReservationDate = new ReservationDate();
        Title = string.Empty;
        AreaId = default!;
        TeacherId = default!;
    }

    public Reservation(string title, DateTime start, DateTime end, int areaId, int teacherId)
    {
        Title = title;
        ReservationDate = new ReservationDate(start, end);
        AreaId = new AreaId(areaId);
        TeacherId = new TeacherId(teacherId);
    }

    public Reservation(CreateReservationCommand command)
    {
        Title = command.Title;
        ReservationDate = new ReservationDate(command.Start, command.End);
        AreaId = new AreaId(command.AreaId);
        TeacherId = new TeacherId(command.TeacherId);
    }

    public int Id { get; }
    public string Title { get; private set; }
    public ReservationDate ReservationDate { get; private set; }
    public AreaId AreaId { get; private set; }
    public TeacherId TeacherId { get; private set; }

    public void UpdateReservationDate(DateTime start, DateTime end)
    {
        ReservationDate = new ReservationDate(start, end);
    }

    public void UpdateTitle(string title)
    {
        Title = title;
    }

    public bool CanReserve(IEnumerable<Reservation> existingReservations)
    {
        return existingReservations.All(r =>
            (ReservationDate.Start < r.ReservationDate.Start || ReservationDate.Start > r.ReservationDate.End) &&
            (ReservationDate.End < r.ReservationDate.Start || ReservationDate.End > r.ReservationDate.End));
    }

    public Reservation Update(UpdateReservationCommand command)
    {
        Title = command.Title;
        ReservationDate = new ReservationDate(command.Start, command.End);
        return this;
    }
}