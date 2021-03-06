using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SenderApp.Services
{
    public class ServiceBusService
    {
        public static int MessagesSent { get; private set; } = 0;

        private string _connectionString;
        private string _queueName;
        private ILogger _logger;

        public ServiceBusService(ILogger logger, IConfiguration configuration)
        {
            _connectionString = configuration["ServiceBus:CONNECTIONSTRING"];
            _queueName = configuration["ServiceBus:QUEUENAME"];

            this._logger = logger;

            Throw.IsNullOrWhiteSpace(nameof(_connectionString), _connectionString);
            Throw.IsNullOrWhiteSpace(nameof(_queueName), _queueName);
        }

        public async Task SendMessageAsync(CancellationToken token, int numberOfMessages)
        {
            using (new PerformanceScope(_logger, nameof(SendMessageAsync)))
            {
                // create a Service Bus client 
                await using (ServiceBusClient client = new ServiceBusClient(_connectionString))
                {
                    // create a sender for the queue 
                    ServiceBusSender sender = client.CreateSender(_queueName);

                    while(!token.IsCancellationRequested && numberOfMessages-- != 0)
                    {
                        using (new PerformanceScope(_logger, $"Sending message"))
                        {
                            // create a message that we can send
                            ServiceBusMessage message = new ServiceBusMessage(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                            // send the message
                            await sender.SendMessageAsync(message);
                            MessagesSent++;
                            _logger.LogInformation($"Message sent: " + MessagesSent);
                        }
                    }
                }
            }
        }
    }
}
