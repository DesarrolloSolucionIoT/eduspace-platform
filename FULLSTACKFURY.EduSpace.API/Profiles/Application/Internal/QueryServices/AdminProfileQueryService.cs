using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.QueryServices;

public class AdminProfileQueryService(IAdminProfileRepository adminProfileRepository) : IAdminProfileQueryService
{
    public async Task<IEnumerable<AdminProfile>> Handle(GetAllAdministratorsProfileQuery query)
    {
        return await adminProfileRepository.ListAsync();
    }

    public async Task<AdminProfile?> Handle(GetAdministratorProfileByIdQuery query)
    {
        return await adminProfileRepository.FindByIdAsync(query.ProfileId);
    }
}