using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.Extensions.Logging;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;

namespace SenderApp.Services
{
    public class EventHubService
    {
        public static int MessagesSent { get; private set; } = 0;

        private string connectionString;
        private string eventHubName;
        private ILogger logger;

        public EventHubService(ILogger logger, IConfiguration configuration)
        {
            connectionString = configuration["EventHub:ConnectionString"];
            eventHubName = configuration["EventHub:EventHubName"];

            this.logger = logger;

            Throw.IsNullOrWhiteSpace(nameof(connectionString), connectionString);
            Throw.IsNullOrWhiteSpace(nameof(eventHubName), eventHubName);
        }

        public async Task SendMessageAsync(CancellationToken token, int numberOfMessages)
        {
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                while (!token.IsCancellationRequested && numberOfMessages-- != 0)
                { 
                    var events = new EventData[] { 
                        new EventData(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"))) 
                    };
                    await producerClient.SendAsync(events);
                    MessagesSent++;
                }

                Console.Write("Published event to Event Hub.");

            }
        }
    }
}
