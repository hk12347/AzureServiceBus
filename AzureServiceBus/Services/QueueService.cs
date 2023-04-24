using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;
using AzureServiceBus.Models;

namespace AzureServiceBus.Services
{
    public class QueueService : IQueueService
    {
        #region "constructor"
        private readonly IConfiguration _config;

        public QueueService(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region "public methods"
        /// <summary>
        /// Send Message to Azure Service Bus
        /// </summary>
        public async Task SendMessageAsync<T>(T serviceBusMessage, string queueName)
        {
            string messageBody = JsonSerializer.Serialize(serviceBusMessage);
            ServiceBusMessage message = new(Encoding.UTF8.GetBytes(messageBody));

            var queueClient = CreateSender(queueName);
            await queueClient.SendMessageAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Receive messages from Azure Service Hub.
        /// </summary>
        public async Task<List<Product>> ReceiveMessages(string queueName)
        {
            List<Product> products = new List<Product>();

            ServiceBusReceiver receiver = CreateReceiver(queueName);
            ServiceBusReceivedMessage? message = null;

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (a, o) =>
            {
                cts.Cancel();
            };

            do
            {
                message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1), cancellationToken: cts.Token);
                if (message != null)
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(message.Body);
                        Product? product = JsonSerializer.Deserialize<Product>(json);
                        products.Add(product);

                        await receiver.CompleteMessageAsync(message, cts.Token);
                    }
                    catch
                    {
                        await receiver.AbandonMessageAsync(message, cancellationToken: cts.Token);
                    }
                }
            }
            while (message != null);
            await receiver.CloseAsync();

            return products;
        }
        #endregion

        #region "private methods"
        /// <summary>
        /// Create Servíce Bus Sender Client
        /// </summary>
        private ServiceBusSender CreateSender(string queueName)
        {
            var serviceBusClient = new ServiceBusClient(_config.GetConnectionString("AzureServiceBus"));
            return serviceBusClient.CreateSender(queueName);
        }

        /// <summary>
        /// Create Servíce Bus Receiver Client
        /// </summary>
        private ServiceBusReceiver CreateReceiver(string queueName)
        {
            var serviceBusClient = new ServiceBusClient(_config.GetConnectionString("AzureServiceBus"));
            return serviceBusClient.CreateReceiver(queueName);
        }
        #endregion
    }
}