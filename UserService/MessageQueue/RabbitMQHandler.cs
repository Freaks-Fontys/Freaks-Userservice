using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Config;

namespace UserService.MessageQueue
{
    public class RabbitMQHandler
    {
        IConnectionFactory factory;
        IConnection connection;
        IModel channel;
        RabbitMQConfig config;


        public RabbitMQHandler(IOptions<RabbitMQConfig> options)
        {
            config = options.Value;
            SetupMQ();
        }

        private void SetupMQ()
        {
            factory = new ConnectionFactory
            {
                Uri = new Uri(config.URI)
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(config.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void SendMessage(object message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish("", config.QueueName, null, body);
        }
    }
}
