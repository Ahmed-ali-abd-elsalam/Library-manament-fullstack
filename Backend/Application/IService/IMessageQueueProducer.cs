namespace Application.IService
{
    public interface IMessageQueueProducer
    {
        Task PublishAsync<T>(string queueName, T message);
    }
}
