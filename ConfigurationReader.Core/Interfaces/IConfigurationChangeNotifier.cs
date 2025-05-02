using System.Threading.Tasks;
using ConfigurationReader.Core.Entities;

namespace ConfigurationReader.Core.Interfaces;

public interface IConfigurationChangeNotifier
{
    Task NotifyChangeAsync(Configuration configuration);
    Task SubscribeAsync(string applicationName);
} 