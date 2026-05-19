using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Design-time factory used exclusively by EF Core migration tooling (dotnet ef migrations add).
/// The connection string is a placeholder — migrations do NOT run against a live DB during generation.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=eduspace_design;User=root;Password=root;");
        return new AppDbContext(optionsBuilder.Options);
    }
}
