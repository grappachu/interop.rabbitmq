using System.Linq;

namespace Grappachu.Interop.RabbitMQ.Samples.Queuing
{
    public class TestPublisher : RabbitMqAbstractPublisher<Message>
    {
        public TestPublisher()
            : base(ReadConfig.GetConfiguration())
        {
        }

        protected override string OnSerializeMessage(Message messageObject, out string contentType)
        {
            var serialized = base.OnSerializeMessage(messageObject, out contentType);
            var reverse = string.Concat(serialized.Reverse());
            return reverse;
        }
    }
}