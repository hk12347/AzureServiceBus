using AzureServiceBus.Models;

namespace AzureServiceBus.Services
{
    public interface IQueueService
    {
        Task SendMessageAsync<T>(T serviceBusMessage, string queueName);
        Task<List<Product>> ReceiveMessages(string queueName);
    }
}