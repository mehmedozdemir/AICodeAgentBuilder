using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AI.CodeAgent.Builder.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances for EF Core tooling.
/// This enables migrations to be created without running the full application.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use SQLite with a default connection string for migrations
        optionsBuilder.UseSqlite("Data Source=aiagentbuilder.db");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
