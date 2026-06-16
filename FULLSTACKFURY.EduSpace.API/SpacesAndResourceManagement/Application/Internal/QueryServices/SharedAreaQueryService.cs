using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.QueryServices;

/// <summary>
///     Classroom query service
/// </summary>
/// <param name="classroomRepository">
///     The classroom repository
/// </param>
public class SharedAreaQueryService(ISharedAreaRepository sharedAreaRepository, ISharedAreaReservationRepository sharedAreaReservationRepository) : ISharedAreaQueryService
{
    /// <Inheritdoc />
    public async Task<SharedArea?> Handle(GetSharedAreaByIdQuery query)
    {
        return await sharedAreaRepository.FindByIdAsync(query.SharedAreaId);
    }

    /// <Inheritdoc />
    public async Task<IEnumerable<SharedArea>> Handle(GetAllSharedAreasQuery query)
    {
        return await sharedAreaRepository.ListAsync();
    }

    public async Task<IEnumerable<SharedAreaReservation>> Handle(GetAllReservationsBySharedAreaIdQuery query)
    {
        return await sharedAreaReservationRepository.FindBySharedAreaIdAndDateAsync(query.SharedAreaId, query.Date);
    }

    public async Task<IEnumerable<SharedAreaReservation>> Handle(GetAllReservationsByTeacherIdQuery query)
    {
        return await sharedAreaReservationRepository.FindByTeacherIdAsync(query.TeacherId);
    }
}