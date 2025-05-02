namespace ConfigurationReader.Core;

public interface IConfigurationReader
{
    T GetValue<T>(string name);
    Task<T> GetValueAsync<T>(string name);
} 