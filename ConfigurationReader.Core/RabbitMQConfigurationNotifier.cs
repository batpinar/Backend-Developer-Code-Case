using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ConfigurationReader.Core.Entities;
using ConfigurationReader.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConfigurationReader.Core;

public class RabbitMQConfigurationNotifier : IConfigurationChangeNotifier, IDisposable
{
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly string _exchangeName = "configuration_changes";
    private readonly ILogger<RabbitMQConfigurationNotifier> _logger;

    public RabbitMQConfigurationNotifier(string hostName, ILogger<RabbitMQConfigurationNotifier> logger)
    {
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to RabbitMQ. Configuration change notifications will be disabled.");
        }
    }

    public async Task NotifyChangeAsync(Configuration configuration)
    {
        if (_channel == null)
        {
            _logger.LogWarning("RabbitMQ connection is not available. Configuration change notification skipped.");
            return;
        }

        try
        {
            var message = JsonSerializer.Serialize(configuration);
            var body = Encoding.UTF8.GetBytes(message);
            
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: "",
                basicProperties: null,
                body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish configuration change notification.");
        }
            
        await Task.CompletedTask;
    }

    public async Task SubscribeAsync(string applicationName)
    {
        if (_channel == null)
        {
            _logger.LogWarning("RabbitMQ connection is not available. Configuration change subscription skipped.");
            return;
        }

        try
        {
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, _exchangeName, "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var configuration = JsonSerializer.Deserialize<Configuration>(message);
                
                if (configuration?.ApplicationName == applicationName)
                {
                    _logger.LogInformation("Configuration changed: {Name}", configuration.Name);
                }
            };

            _channel.BasicConsume(queue: queueName,
                                autoAck: true,
                                consumer: consumer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to configuration changes.");
        }
                            
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
} 