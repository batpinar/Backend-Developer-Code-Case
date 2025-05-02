using System;
using System.Threading.Tasks;
using ConfigurationReader.Core;
using ConfigurationReader.Core.Entities;
using ConfigurationReader.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace ConfigurationReader.Tests;

public class ConfigurationReaderTests
{
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IConfigurationChangeNotifier> _mockChangeNotifier;
    private readonly ConfigurationReader _configurationReader;
    private readonly IMemoryCache _cache;
    private const string ApplicationName = "TestApp";

    public ConfigurationReaderTests()
    {
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockChangeNotifier = new Mock<IConfigurationChangeNotifier>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        
        _configurationReader = new ConfigurationReader(
            "TestApp",
            "TestConnectionString",
            1000,
            _mockConfigurationService.Object,
            _mockChangeNotifier.Object
        );
    }

    [Fact]
    public async Task GetValue_WhenKeyExists_ReturnsValue()
    {
        // Arrange
        var expectedValue = "TestValue";
        var configuration = new Configuration
        {
            Name = "TestKey",
            Type = "System.String",
            Value = expectedValue,
            IsActive = true,
            ApplicationName = "TestApp"
        };

        _mockConfigurationService
            .Setup(x => x.GetByNameAsync("TestKey", "TestApp"))
            .ReturnsAsync(configuration);

        // Act
        var result = _configurationReader.GetValue<string>("TestKey");

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetValue_WhenKeyDoesNotExist_ThrowsException()
    {
        // Arrange
        _mockConfigurationService
            .Setup(x => x.GetByNameAsync("NonExistentKey", "TestApp"))
            .ReturnsAsync((Configuration)null);

        // Act & Assert
        Assert.Throws<ConfigurationException>(() => 
            _configurationReader.GetValue<string>("NonExistentKey"));
    }

    [Fact]
    public void GetValue_WhenConfigurationIsInactive_ThrowsException()
    {
        // Arrange
        var configuration = new Configuration
        {
            Name = "TestKey",
            Type = "System.String",
            Value = "TestValue",
            IsActive = false,
            ApplicationName = "TestApp"
        };

        _mockConfigurationService
            .Setup(x => x.GetByNameAsync("TestKey", "TestApp"))
            .ReturnsAsync(configuration);

        // Act & Assert
        Assert.Throws<ConfigurationException>(() => 
            _configurationReader.GetValue<string>("TestKey"));
    }

    [Fact]
    public async Task GetValueAsync_WhenConfigurationExists_ReturnsValue()
    {
        // Arrange
        var config = new Configuration
        {
            Name = "TestConfig",
            Type = "string",
            Value = "TestValue",
            ApplicationName = ApplicationName,
            IsActive = true
        };

        _mockConfigurationService.Setup(x => x.GetByNameAsync("TestConfig", ApplicationName))
            .ReturnsAsync(config);

        // Act
        var result = await _configurationReader.GetValueAsync<string>("TestConfig");

        // Assert
        Assert.Equal("TestValue", result);
    }

    [Fact]
    public async Task GetValueAsync_WhenConfigurationNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockConfigurationService.Setup(x => x.GetByNameAsync("NonExistentConfig", ApplicationName))
            .ReturnsAsync((Configuration?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _configurationReader.GetValueAsync<string>("NonExistentConfig"));
    }

    [Fact]
    public async Task GetValueAsync_WhenStorageUnavailable_UsesCachedValue()
    {
        // Arrange
        var config = new Configuration
        {
            Name = "TestConfig",
            Type = "string",
            Value = "TestValue",
            ApplicationName = ApplicationName,
            IsActive = true
        };

        _mockConfigurationService.Setup(x => x.GetByNameAsync("TestConfig", ApplicationName))
            .ReturnsAsync(config)
            .Verifiable();

        // First call to populate cache
        await _configurationReader.GetValueAsync<string>("TestConfig");

        // Setup service to throw on second call
        _mockConfigurationService.Setup(x => x.GetByNameAsync("TestConfig", ApplicationName))
            .ThrowsAsync(new Exception("Storage unavailable"));

        // Act
        var result = await _configurationReader.GetValueAsync<string>("TestConfig");

        // Assert
        Assert.Equal("TestValue", result);
        _mockConfigurationService.Verify(x => x.GetByNameAsync("TestConfig", ApplicationName), Times.Exactly(2));
    }
} 