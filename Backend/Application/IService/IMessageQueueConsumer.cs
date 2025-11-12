namespace Application.IService
{
    public interface IMessageQueueConsumer
    {
        Task ConsumeAsync<T>(string queueName, Func<T, Task> onMessage);
    }
}
