using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RabbitMQMailServer.ConfigurationSections
{
    [ConfigurationCollection(typeof(RabbitMqMailServerQueuesConfigurationElement))]
    public class RabbitMqMailServerQueuesConfigurationElementCollection: ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RabbitMqMailServerQueuesConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var elm = element as RabbitMqMailServerQueuesConfigurationElement;

            if (elm == null)
            {
                throw new Exception();
            }

            return elm.Name;
        }

        public IEnumerable<RabbitMqMailServerQueuesConfigurationElement> GetAll()
        {
            return BaseGetAllKeys()
                .ToList()
                .Select(x => base.BaseGet(x) as RabbitMqMailServerQueuesConfigurationElement);
        }
    }
}