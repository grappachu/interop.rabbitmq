using System.Configuration;

namespace Grappachu.Interop.RabbitMQ.Samples.Queuing
{
    internal static class ReadConfig
    {
        public static RabbitMqSettings GetConfiguration()
        {
            return new RabbitMqSettings
            {
                HostName = ConfigurationManager.AppSettings["RabbitMQ.HostName"],
                User = ConfigurationManager.AppSettings["RabbitMQ.User"],
                Password = ConfigurationManager.AppSettings["RabbitMQ.Password"],
                QueueName = ConfigurationManager.AppSettings["RabbitMQ.QueueName"],
                ExchangeName = ConfigurationManager.AppSettings["RabbitMQ.ExchangeName"],
                RoutingKey = ConfigurationManager.AppSettings["RabbitMQ.RoutingKey"],
                VirtualHost = ConfigurationManager.AppSettings["RabbitMQ.VirtualHost"],
                Port = int.Parse(ConfigurationManager.AppSettings["RabbitMQ.Port"])
            };
        }
    }
}