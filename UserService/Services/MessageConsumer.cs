using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using UserService.Interfaces;

namespace UserService.Services
{
    public class MessageConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: "UserRegistered",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                // Handle the message
                Console.WriteLine("Received message: " + message);
            };
            _channel.BasicConsume(
                queue: "UserRegistered",
                autoAck: true,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
