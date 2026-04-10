using FULLSTACKFURY.EduSpace.API.EventsScheduling.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Application.Internal.CommandServices;

public class ReservationCommandService(
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    IExternalProfileService externalProfileService) : IReservationCommandService

{
    public async Task<Reservation?> Handle(CreateReservationCommand command)
    {
        if (!externalProfileService.ValidateTeacherIdExistence(command.TeacherId))
            throw new Exception("Teacher does not exist");
        //TODO: validte the area id
        //Can improve this using static methods for command creation and use custom domain exceptions
        var reservation = new Reservation(command);
        try
        {
            var reservationsOfTheDay =
                await reservationRepository.FindAllByAreaIdMonthAndDayAsync(command.AreaId, command.Start.Month,
                    command.Start.Day);
            var reservationsOfTheDayList = reservationsOfTheDay.ToList();

            if (!reservation.CanReserve(reservationsOfTheDayList))
                throw new Exception("Reservation cannot be made. Area is already reserved for the given time");

            await reservationRepository.AddAsync(reservation);
            await unitOfWork.CompleteAsync();
            return reservation;
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while creating the reservation: {e.Message}");
        }
    }

    public async Task<Reservation?> Handle(UpdateReservationCommand command)
    {
        var reservation = await reservationRepository.FindByIdAsync(command.Id);
        if (reservation == null) throw new ArgumentException($"Reservation with ID {command.Id} not found.");

        // Validate time conflicts
        var reservationsOfTheDay = await reservationRepository.FindAllByAreaIdMonthAndDayAsync(
            reservation.AreaId.Identifier, command.Start.Month, command.Start.Day);

        var otherReservations = reservationsOfTheDay.Where(r => r.Id != command.Id).ToList();
        var tempReservation = new Reservation(command.Title, command.Start, command.End, reservation.AreaId.Identifier,
            reservation.TeacherId.TeacherIdentifier);

        if (!tempReservation.CanReserve(otherReservations))
            throw new Exception("Reservation cannot be updated. Time slot conflicts with existing reservations.");

        reservation.Update(command);
        reservationRepository.Update(reservation);
        await unitOfWork.CompleteAsync();

        return reservation;
    }

    public async Task Handle(DeleteReservationCommand command)
    {
        var reservation = await reservationRepository.FindByIdAsync(command.ReservationId);
        if (reservation == null) throw new ArgumentException("The reservation does not exist");

        reservationRepository.Remove(reservation);
        await unitOfWork.CompleteAsync();
    }
}