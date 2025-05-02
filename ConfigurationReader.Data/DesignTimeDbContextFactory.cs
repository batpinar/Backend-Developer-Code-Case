using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConfigurationReader.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
{
    public ConfigurationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=ConfigurationReader;Username=postgres;Password=postgres");

        return new ConfigurationDbContext(optionsBuilder.Options);
    }
} 