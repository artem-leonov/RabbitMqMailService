using System.Configuration;
using RabbitMQMailServer.ConfigurationSections;

namespace RabbitMQMailServer.Configuration
{
    public class RabbitMqMailServerConfigurationSection: ConfigurationSection
    {
        private const string QueuesPropertyName = "queues";
        private const string HostPropertyName = "host";
        private const string WorkersCountPropertyName = "workersCount";
        private const string UsernamePropertyName = "username";
        private const string PasswordPropertyName = "password";
        private const string VirtualHostPropertyName = "virtualHost";

        [ConfigurationProperty(QueuesPropertyName, IsRequired = true)]
        public RabbitMqMailServerQueuesConfigurationElementCollection Queues { get { return (RabbitMqMailServerQueuesConfigurationElementCollection) base[QueuesPropertyName];} }

        [ConfigurationProperty(HostPropertyName, IsRequired = false, DefaultValue = "localhost")]
        public string Host { get { return (string) this[HostPropertyName]; } }

        [ConfigurationProperty(WorkersCountPropertyName, IsRequired = false, DefaultValue = 1)]
        public int WorkersCount { get { return (int)this[WorkersCountPropertyName]; } }

        [ConfigurationProperty(UsernamePropertyName, IsRequired = false, DefaultValue = null)]
        public string Username { get { return (string)this[UsernamePropertyName]; } }

        [ConfigurationProperty(PasswordPropertyName, IsRequired = false, DefaultValue = null)]
        public string Password { get { return (string)this[PasswordPropertyName]; } }

        [ConfigurationProperty(VirtualHostPropertyName, IsRequired = false, DefaultValue = "/")]
        public string VirtualHost { get { return (string)this[VirtualHostPropertyName]; } }


    }
}