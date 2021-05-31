using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace SenderApp.Services
{
    public class ServiceBusService
    {
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

        public async Task SendMessageAsync()
        {
            using (this._logger.BeginScope(nameof(SendMessageAsync)))
            {
                // create a Service Bus client 
                await using (ServiceBusClient client = new ServiceBusClient(_connectionString))
                {
                    // create a sender for the queue 
                    ServiceBusSender sender = client.CreateSender(_queueName);

                    for (int i = 0; i < 1000; i++)
                    {
                        using (_logger.BeginScope($"Sending message " + i))
                        {
                            // create a message that we can send
                            ServiceBusMessage message = new ServiceBusMessage("Hello world!");

                            // send the message
                            await sender.SendMessageAsync(message);

                            _logger.LogInformation($"Message sent: " + i );
                        }
                    }
                }
            }
        }
    }
}
