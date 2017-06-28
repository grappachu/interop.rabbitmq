using System;
using System.Linq;
using System.Text;
using Grappachu.Core.Messaging;
using Grappachu.Core.Preview.UI;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Grappachu.Interop.RabbitMQ.Samples.Queuing
{
    public class TestConsumer : RabbitMqAbstractConsumer<Message>
    {
        public TestConsumer()
            : base(ReadConfig.GetConfiguration())
        {
        }

        protected override Message OnDeserializingMessage(string messageContentRaw, string contentType)
        {
            switch (contentType)
            {
                case "application/json":
                    return base.OnDeserializingMessage(messageContentRaw, contentType);
                default:
                    return Message.Parse(messageContentRaw);
            }
        }


        protected override string OnReadingMessage(BasicDeliverEventArgs e)
        {
            var encoding = e.BasicProperties.IsContentEncodingPresent()
                ? Encoding.GetEncoding(e.BasicProperties.ContentEncoding)
                : Encoding.Default;
            var messageRaw = encoding.GetString(e.Body);
            var reversed = string.Concat(messageRaw.Reverse());
            return reversed;
        }

        protected override void OnError(Exception exception)
        {
            Dialogs.ShowError(exception.Message);
        }


        protected override void OnMessageConsumed(IModel channel, Envelope<Message> qm, BasicDeliverEventArgs e)
        {
            base.OnMessageConsumed(channel, qm, e);

            var diff = DateTime.Now.Subtract(qm.Message.SentDate);
            Console.WriteLine(@"Processed after {0} mills from sent", diff.TotalMilliseconds);
        }
    }
}