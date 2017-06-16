namespace Grappachu.Interop.RabbitMQ
{
    /// <summary>
    ///     Defines a wrapper used to send messages through RabbitMQ
    /// </summary>
    public class RabbitMqMessageRequest
    {
        /// <summary>
        ///     Gets or sets the serialized message to be sent
        /// </summary>
        public string ContentData { get; set; }

        /// <summary>
        ///     Gets or sets the content type to be used when sending this message
        /// </summary>
        public string ContentType { get; set; }
    }
}