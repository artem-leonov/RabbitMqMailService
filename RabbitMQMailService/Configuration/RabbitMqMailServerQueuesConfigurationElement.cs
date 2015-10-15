using System.Configuration;

namespace RabbitMQMailServer.ConfigurationSections
{
    public class RabbitMqMailServerQueuesConfigurationElement: ConfigurationElement
    {
        private const string NamePropertyName = "name";
        private const string PriorityPropertyName = "priority";

        [ConfigurationProperty(NamePropertyName, IsRequired = true, IsKey = true)]
        public string Name { get { return (string) base[NamePropertyName]; } }

        [ConfigurationProperty(PriorityPropertyName, IsRequired = false, DefaultValue = 0)]
        public int Priority { get { return (int) base[PriorityPropertyName]; } }
    }
}