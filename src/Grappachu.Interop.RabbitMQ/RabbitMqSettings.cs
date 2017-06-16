namespace Grappachu.Interop.RabbitMQ
{
    /// <summary>
    /// Define a set of parameters to configure a connection to a RabbitMQ Exchange
    /// </summary>
    public class RabbitMqSettings
    {
        /// <summary>
        /// Name of server
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Port to be used. If null the system default will be used
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// username to connect the queue
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// password to connect the queue
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// the name of the Exchange
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// the name of the queue
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// virtual host path
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// routing key
        /// </summary>
        public string RoutingKey { get; set; }
    }
}