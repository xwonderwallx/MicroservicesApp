using RabbitMQ.Client;
using System.Text;

namespace AuthService.Services
{
    public class MessagePublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessagePublisher()
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

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "",
                routingKey: "UserRegistered",
                basicProperties: null,
                body: body
            );
        }
    }
}
