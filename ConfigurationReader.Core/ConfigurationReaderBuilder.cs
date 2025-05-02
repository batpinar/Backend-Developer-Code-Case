using System;
using ConfigurationReader.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ConfigurationReader.Core;

public class ConfigurationReaderBuilder
{
    private IServiceProvider? _serviceProvider;
    private IMemoryCache? _cache;
    private ILogger<ConfigurationReader>? _logger;
    private string _applicationName = "ConfigurationReader.Web";
    private TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public ConfigurationReaderBuilder SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        return this;
    }

    public ConfigurationReaderBuilder SetCache(IMemoryCache cache)
    {
        _cache = cache;
        return this;
    }

    public ConfigurationReaderBuilder SetLogger(ILogger<ConfigurationReader> logger)
    {
        _logger = logger;
        return this;
    }

    public ConfigurationReaderBuilder SetApplicationName(string applicationName)
    {
        _applicationName = applicationName;
        return this;
    }

    public ConfigurationReaderBuilder SetCacheExpiration(TimeSpan cacheExpiration)
    {
        _cacheExpiration = cacheExpiration;
        return this;
    }

    public ConfigurationReader Build()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceProvider is required.");
        if (_cache == null)
            throw new InvalidOperationException("Cache is required.");
        if (_logger == null)
            throw new InvalidOperationException("Logger is required.");

        return new ConfigurationReader(
            _serviceProvider,
            _cache,
            _logger,
            _applicationName,
            _cacheExpiration
        );
    }
} 