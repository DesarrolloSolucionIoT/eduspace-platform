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
public class ClassroomQueryService(IClassroomRepository classroomRepository) : IClassroomQueryService
{
    /// <inheritdoc />
    public async Task<Classroom?> Handle(GetClassroomByIdQuery query)
    {
        return await classroomRepository.FindByIdAsync(query.ClassroomId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Classroom>> Handle(GetAllClassroomsQuery query)
    {
        return await classroomRepository.ListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Classroom>> Handle(GetAllClassroomsByTeacherIdQuery query)
    {
        return await classroomRepository.FindByTeacherIdAsync(query.TeacherId);
    }
}