using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQMailServer.ConfigurationSections;

namespace RabbitMQMailServer.Interfaces
{
    public interface IMailServiceWorker
    {
        void DoWork(ConnectionFactory factory, IEnumerable<RabbitMqMailServerQueuesConfigurationElement> queues);

        Task Stop();
    }
}