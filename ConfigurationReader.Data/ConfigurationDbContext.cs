using ConfigurationReader.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader.Data;

public class ConfigurationDbContext : DbContext
{
    public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Configuration> Configurations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.ApplicationName).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
        });
    }
} 