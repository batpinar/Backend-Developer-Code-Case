using ConfigurationReader.Core.Entities;
using ConfigurationReader.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConfigurationReader.Core;

public class NoOpConfigurationNotifier : IConfigurationChangeNotifier
{
    private readonly ILogger<NoOpConfigurationNotifier> _logger;

    public NoOpConfigurationNotifier(ILogger<NoOpConfigurationNotifier> logger)
    {
        _logger = logger;
    }

    public Task NotifyChangeAsync(Configuration configuration)
    {
        _logger.LogInformation("Configuration change notification disabled (NoOp notifier)");
        return Task.CompletedTask;
    }

    public Task SubscribeAsync(string applicationName)
    {
        _logger.LogInformation("Configuration subscription disabled (NoOp notifier) for application: {ApplicationName}", applicationName);
        return Task.CompletedTask;
    }
} 