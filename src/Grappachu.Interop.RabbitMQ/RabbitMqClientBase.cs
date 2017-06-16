using RabbitMQ.Client;

namespace Grappachu.Interop.RabbitMQ
{
    /// <summary>
    ///     Defines a basic client for both publisher and consumer
    /// </summary>
    public abstract class RabbitMqClientBase
    {
        /// <summary>
        ///     Gets the settings collection used by for this client
        /// </summary>
        protected readonly RabbitMqSettings Settings;

        /// <summary>
        ///     Acrivates a new instance of <see cref="RabbitMqClientBase" />
        /// </summary>
        /// <param name="settings"></param>
        protected RabbitMqClientBase(RabbitMqSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        ///     Gets a new connection factory for this client
        /// </summary>
        /// <returns></returns>
        protected IConnectionFactory GetConnectionFactory()
        {
            var factory = new ConnectionFactory
            {
                UserName = Settings.User,
                Password = Settings.Password,
                VirtualHost = Settings.VirtualHost,
                HostName = Settings.HostName
            };
            return factory;
        }
    }
}