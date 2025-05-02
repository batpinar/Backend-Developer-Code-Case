using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationReader.Core.Entities;

namespace ConfigurationReader.Core.Interfaces;

public interface IConfigurationService
{
    Task<Configuration?> GetByNameAsync(string name, string applicationName);
    Task<IEnumerable<Configuration>> GetAllAsync(string applicationName);
    Task<Configuration?> GetByIdAsync(int id);
    Task<Configuration> AddAsync(Configuration configuration);
    Task<Configuration> UpdateAsync(Configuration configuration);
    Task DeleteAsync(int id);
} 