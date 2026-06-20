using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;

/// <summary>
/// Shared Area Reservation Repository Interface
/// </summary>

public interface ISharedAreaReservationRepository : IBaseRepository<SharedAreaReservation>
{
    /// <summary>
    /// Verifies if a reservation exists for the specified shared area and date/time range.
    /// </summary>
    /// <param name="sharedAreaId">The shared area identifier.</param>
    /// <param name="date">The date of the reservation.</param>
    /// <param name="startTime">The start time of the reservation.</param>
    /// <param name="endTime">The end time of the reservation.</param>
    /// <returns>A boolean value indicating whether the reservation exists.</returns>
    Task<bool> ExistsBySharedAreaAndDateTimeRangeAsync(int sharedAreaId, DateOnly date,
    TimeOnly startTime, TimeOnly endTime);

    /// <summary>
    /// Finds reservations for a specific shared area and date.
    /// </summary>
    /// <param name="sharedAreaId">The shared area identifier.</param>
    /// <param name="date">The date of the reservation.</param>
    /// <returns>A result containing a collection of reservations for the specified shared area and date.</returns>
    Task<IEnumerable<SharedAreaReservation>> FindBySharedAreaIdAndDateAsync(int sharedAreaId
    , DateOnly date);

    /// <summary>
    /// Finds reservations for a specific teacher.
    /// </summary> 
    /// <param name="teacherId">The teacher identifier.</param>
    /// <returns>A result containing a collection of reservations for the specified teacher.</returns>
    Task<IEnumerable<SharedAreaReservation>> FindByTeacherIdAsync(int teacherId);
}