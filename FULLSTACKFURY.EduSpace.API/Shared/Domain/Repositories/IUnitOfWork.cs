namespace FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

public interface IUnitOfWork
{
    Task CompleteAsync();
}