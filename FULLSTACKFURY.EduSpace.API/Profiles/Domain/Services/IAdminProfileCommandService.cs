using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;

public interface IAdminProfileCommandService
{
    Task<AdminProfile?> Handle(CreateAdministratorProfileCommand command);
    Task<AdminProfile?> Handle(UpdateAdminProfileCommand command);
    Task Handle(DeleteAdminProfileCommand command);
}