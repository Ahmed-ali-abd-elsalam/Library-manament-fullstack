using Application.IService;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.RabbitMq
{
    public class RabbitMqProducer : IMessageQueueProducer
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqProducer(IConfiguration configuration)
        {
            _factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            var connection = await _factory.CreateConnectionAsync();
            try
            {
                var channel = await connection.CreateChannelAsync();
                try
                {
                    // Declare the queue (idempotent operation)
                    await channel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    // Serialize the message
                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    // Publish the message
                    await channel.BasicPublishAsync(
                        exchange: string.Empty,
                        routingKey: queueName,
                        body: body
                    );
                }
                finally
                {
                    await channel.CloseAsync();
                    channel.Dispose();
                }
            }
            finally
            {
                await connection.CloseAsync();
                connection.Dispose();
            }
        }
    }
}