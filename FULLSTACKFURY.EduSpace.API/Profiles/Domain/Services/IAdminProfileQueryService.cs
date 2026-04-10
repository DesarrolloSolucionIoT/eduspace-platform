using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;

public interface IAdminProfileQueryService
{
    Task<IEnumerable<AdminProfile>> Handle(GetAllAdministratorsProfileQuery query);
    Task<AdminProfile?> Handle(GetAdministratorProfileByIdQuery query);
}