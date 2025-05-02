using ConfigurationReader.Core;
using ConfigurationReader.Core.Entities;
using ConfigurationReader.Core.Interfaces;
using ConfigurationReader.Data;
using ConfigurationReader.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Add DbContext
services.AddDbContext<ConfigurationDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=ConfigurationReader;Username=postgres;Password=postgres"));

// Add ConfigurationService
services.AddScoped<IConfigurationService, ConfigurationService>();

var builder = new ConfigurationReaderBuilder(services)
    .UseConnectionString("Host=localhost;Database=ConfigurationReader;Username=postgres;Password=postgres")
    .SetApplicationName("ConsoleApp")
    .SetCacheExpiration(TimeSpan.FromMinutes(5));

builder.Build();

var serviceProvider = services.BuildServiceProvider();
var configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
var configurationReader = serviceProvider.GetRequiredService<IConfigurationReader>();

// Add a sample configuration
var config = new Configuration
{
    Name = "SampleInt",
    Type = "System.Int32",
    Value = "42",
    ApplicationName = "ConsoleApp",
    IsActive = true
};

await configurationService.AddConfigurationAsync(config);

// Read the configuration value
var value = configurationReader.GetValue<int>("SampleInt");
Console.WriteLine($"Configuration value: {value}");

// Read asynchronously
var asyncValue = await configurationReader.GetValueAsync<int>("SampleInt");
Console.WriteLine($"Async configuration value: {asyncValue}");
