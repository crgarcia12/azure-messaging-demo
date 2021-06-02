using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace receiver
{
    public static class receiver
    {
        [FunctionName("receiver")]
        public static void Run([ServiceBusTrigger("messagesqueue", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {   
            //Comment2
            log.LogInformation($"[QueueMessage];{myQueueItem};{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
        }
    }
}
