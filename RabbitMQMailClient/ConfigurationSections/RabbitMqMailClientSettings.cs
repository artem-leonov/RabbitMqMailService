using System.Configuration;

namespace RabbitMQMailClient.ConfigurationSections
{
    public class RabbitMqMailClientSettings : ConfigurationSection
    {
        private const string QueuePropertyName = "queue";
        private const string HostPropertyName = "host";
        private const string UsernamePropertyName = "username";
        private const string PasswordPropertyName = "password";
        private const string VirtualHostPropertyName = "virtualHost";

        [ConfigurationProperty(QueuePropertyName, IsRequired = true)]
        public string Queue { get { return (string) this[QueuePropertyName]; } }

        [ConfigurationProperty(HostPropertyName, IsRequired = false, DefaultValue = "localhost")]
        public string Host { get { return (string) this[HostPropertyName]; } }

        [ConfigurationProperty(UsernamePropertyName, IsRequired = false, DefaultValue = null)]
        public string Username { get { return (string)this[UsernamePropertyName]; } }

        [ConfigurationProperty(PasswordPropertyName, IsRequired = false, DefaultValue = null)]
        public string Password { get { return (string)this[PasswordPropertyName]; } }

        [ConfigurationProperty(VirtualHostPropertyName, IsRequired = false, DefaultValue = "/")]
        public string VirtualHost { get { return (string)this[VirtualHostPropertyName]; } }
    }
}