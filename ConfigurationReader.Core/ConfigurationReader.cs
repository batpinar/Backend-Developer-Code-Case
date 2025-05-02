using System;
using System.Threading.Tasks;
using ConfigurationReader.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConfigurationReader.Core;

public class ConfigurationReader
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ConfigurationReader> _logger;
    private readonly string _applicationName;
    private readonly TimeSpan _cacheExpiration;

    public ConfigurationReader(
        IServiceProvider serviceProvider,
        IMemoryCache cache,
        ILogger<ConfigurationReader> logger,
        string applicationName,
        TimeSpan cacheExpiration)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
        _logger = logger;
        _applicationName = applicationName;
        _cacheExpiration = cacheExpiration;
    }

    public async Task<T?> GetValueAsync<T>(string key)
    {
        try
        {
            var cacheKey = $"{_applicationName}:{key}";
            
            if (_cache.TryGetValue(cacheKey, out T? cachedValue))
            {
                return cachedValue;
            }

            using var scope = _serviceProvider.CreateScope();
            var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
            
            var configuration = await configurationService.GetByNameAsync(key, _applicationName);
            if (configuration == null)
            {
                return default;
            }

            var value = Convert.ChangeType(configuration.Value, typeof(T));
            _cache.Set(cacheKey, value, _cacheExpiration);
            
            return (T?)value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configuration value for key {Key}", key);
            return default;
        }
    }

    public async Task RefreshCache()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
            
            var configurations = await configurationService.GetAllAsync(_applicationName);
            foreach (var config in configurations)
            {
                var cacheKey = $"{_applicationName}:{config.Name}";
                var value = Convert.ChangeType(config.Value, Type.GetType(config.Type) ?? typeof(string));
                _cache.Set(cacheKey, value, _cacheExpiration);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing configuration cache");
        }
    }
} 