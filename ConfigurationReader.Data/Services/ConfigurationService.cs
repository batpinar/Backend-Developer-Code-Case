using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationReader.Core.Entities;
using ConfigurationReader.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader.Data.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly ConfigurationDbContext _context;
    private readonly IConfigurationChangeNotifier _changeNotifier;

    public ConfigurationService(ConfigurationDbContext context, IConfigurationChangeNotifier changeNotifier)
    {
        _context = context;
        _changeNotifier = changeNotifier;
    }

    public async Task<Configuration?> GetByNameAsync(string name, string applicationName)
    {
        return await _context.Configurations
            .FirstOrDefaultAsync(c => c.Name == name && c.ApplicationName == applicationName);
    }

    public async Task<IEnumerable<Configuration>> GetAllAsync(string applicationName)
    {
        return await _context.Configurations
            .Where(c => c.ApplicationName == applicationName)
            .ToListAsync();
    }

    public async Task<Configuration?> GetByIdAsync(int id)
    {
        return await _context.Configurations.FindAsync(id);
    }

    public async Task<Configuration> AddAsync(Configuration configuration)
    {
        configuration.CreatedDate = DateTime.UtcNow;
        _context.Configurations.Add(configuration);
        await _context.SaveChangesAsync();
        await _changeNotifier.NotifyChangeAsync(configuration);
        return configuration;
    }

    public async Task<Configuration> UpdateAsync(Configuration configuration)
    {
        var existingConfig = await _context.Configurations.FindAsync(configuration.Id);
        if (existingConfig == null)
            throw new KeyNotFoundException($"Configuration with ID {configuration.Id} not found.");

        existingConfig.Name = configuration.Name;
        existingConfig.Type = configuration.Type;
        existingConfig.Value = configuration.Value;
        existingConfig.IsActive = configuration.IsActive;
        existingConfig.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _changeNotifier.NotifyChangeAsync(existingConfig);
        return existingConfig;
    }

    public async Task DeleteAsync(int id)
    {
        var configuration = await _context.Configurations.FindAsync(id);
        if (configuration == null)
            throw new KeyNotFoundException($"Configuration with ID {id} not found.");

        _context.Configurations.Remove(configuration);
        await _context.SaveChangesAsync();
        await _changeNotifier.NotifyChangeAsync(configuration);
    }
} 