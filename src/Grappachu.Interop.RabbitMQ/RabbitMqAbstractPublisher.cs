using System.Text;
using Grappachu.Core.Messaging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Grappachu.Interop.RabbitMQ
{
    /// <summary>
    ///     This is the base class to inherit from when creating a publihser based on RabbitMQ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RabbitMqAbstractPublisher<T> : RabbitMqClientBase, IPublisher<T>
    {
        /// <summary>
        ///     Activates a new instance of <see cref="RabbitMqAbstractPublisher{T}" />
        /// </summary>
        /// <param name="settings"></param>
        protected RabbitMqAbstractPublisher(RabbitMqSettings settings)
            : base(settings)
        {
        }

        /// <inheritdoc />
        public void Publish(T message)
        {
            string messageConentType;
            var messageString = OnSerializeMessage(message, out messageConentType);

            var msg = new RabbitMqMessageRequest
            {
                ContentData = messageString,
                ContentType = messageConentType
            };

            using (var connection = OnCreateConnection())
            {
                OnPublishing(msg, connection);
            }
        }

        private IConnection OnCreateConnection()
        {
            var factory = GetConnectionFactory();
            var mqConnection = factory.CreateConnection();
            return mqConnection;
        }

        /// <summary>
        ///     Serializes the message to a string before sending and sets its content type
        /// </summary>
        /// <param name="messageObject"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        protected virtual string OnSerializeMessage(T messageObject, out string contentType)
        {
            var messageJson = JsonConvert.SerializeObject(messageObject);
            contentType = "application/json";
            return messageJson;
        }

        /// <summary>
        ///     Publishes a message on the exchange
        /// </summary>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        protected virtual void OnPublishing(RabbitMqMessageRequest message, IConnection connection)
        {
            var exchange = Settings.ExchangeName;
            var queue = Settings.QueueName;
            var routing = Settings.RoutingKey;

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, ExchangeType.Direct);
                channel.QueueDeclare(queue, true, false, false, null);
                channel.QueueBind(queue, exchange, routing, null);

                var props = channel.CreateBasicProperties();
                props.ContentEncoding = Encoding.UTF8.HeaderName;
                props.ContentType = message.ContentType;
                props.DeliveryMode = 2;

                var messageBodyBytes = Encoding.UTF8.GetBytes(message.ContentData);

                channel.BasicPublish(exchange, routing, props, messageBodyBytes);
            }
        }

       
    }
}