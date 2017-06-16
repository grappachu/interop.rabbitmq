using System;
using System.Text;
using Grappachu.Core.Messaging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Grappachu.Interop.RabbitMQ
{
    /// <summary>
    ///     This is the base class to inherit from when creating a consumer based on RabbitMQ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RabbitMqAbstractConsumer<T> : RabbitMqClientBase, IConsumer<T>
    {
        private IModel _channel;
        private IConnection _connection;

        /// <summary>
        ///     Acrivates a new instance of <see cref="RabbitMqAbstractConsumer{T}" />
        /// </summary>
        /// <param name="rabbitMqSettings"></param>
        protected RabbitMqAbstractConsumer(RabbitMqSettings rabbitMqSettings)
            : base(rabbitMqSettings)
        {
        }

        /// <inheritdoc />
        public event EventHandler<Envelope<T>> MessageReceived;

        /// <inheritdoc />
        public void BeginConsume()
        {
            var factory = GetConnectionFactory();
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(Settings.QueueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ProcessSingleMessage;

            _channel.BasicConsume(Settings.QueueName, false, consumer);
        }

        /// <inheritdoc />
        public void EndConsume()
        {
            _channel.Abort();
            _channel?.Dispose();
            _connection?.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void ProcessSingleMessage(object sender, BasicDeliverEventArgs e)
        {
            if (MessageReceived == null)
                return;
            try
            {
                var channel = ((EventingBasicConsumer) sender).Model;
                var messageJson = OnReadingMessage(e);
                var messageContentType = e.BasicProperties?.ContentType;
                var messageItem = OnDeserializingMessage(messageJson, messageContentType);
                var qm = new Envelope<T>(e.DeliveryTag.ToString(), messageItem);

                OnMessageConsuming(qm);

                OnMessageConsumed(channel, qm, e);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }


        /// <summary>
        ///     Executes the basic Ack or Nack policies after a message is consumed
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="qm"></param>
        /// <param name="e"></param>
        protected virtual void OnMessageConsumed(IModel channel, Envelope<T> qm, BasicDeliverEventArgs e)
        {
            if (qm.GiveAck.HasValue)
                if (qm.GiveAck.Value)
                    channel.BasicAck(e.DeliveryTag, true);
                else
                    channel.BasicNack(e.DeliveryTag, true, false);
        }

        /// <summary>
        ///     Gets the raw message received data
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual string OnReadingMessage(BasicDeliverEventArgs e)
        {
            var encoding = e.BasicProperties.IsContentEncodingPresent()
                ? Encoding.GetEncoding(e.BasicProperties.ContentEncoding)
                : Encoding.Default;
            var messageRaw = encoding.GetString(e.Body);
            return messageRaw;
        }

        /// <summary>
        ///     Deserialize the received message into {T}
        /// </summary>
        /// <param name="messageContentRaw"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        protected virtual T OnDeserializingMessage(string messageContentRaw, string contentType)
        {
            var messageItem = JsonConvert.DeserializeObject<T>(messageContentRaw);
            return messageItem;
        }

        /// <summary>
        ///     Runs when an error occurs during message consumption
        /// </summary>
        /// <param name="exception"></param>
        protected virtual void OnError(Exception exception)
        {
            throw exception;
        }

        /// <summary>
        ///     Closes channel and release resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_channel != null)
                {
                    _channel.Dispose();
                    _channel = null;
                }
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        /// <summary>
        ///     Raises the event <see cref="MessageReceived" /> to execute custom user logic
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMessageConsuming(Envelope<T> e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}