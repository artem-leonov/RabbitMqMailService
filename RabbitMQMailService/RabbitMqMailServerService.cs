using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client;
using RabbitMQMailServer.Configuration;
using RabbitMQMailServer.ConfigurationSections;
using RabbitMQMailServer.Interfaces;

namespace RabbitMQMailServer
{
    public partial class RabbitMqMailServerService : ServiceBase
    {
        private readonly Logger _logger;
        private readonly RabbitMqMailServerConfigurationSection _mailServiceSettings;
        private readonly IList<IMailServiceWorker> _workers; 

        public RabbitMqMailServerService()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _mailServiceSettings = ConfigurationManager.GetSection("rabbitMqMailServer") as RabbitMqMailServerConfigurationSection;
            _workers = new List<IMailServiceWorker>();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("Service starting...");
            Thread.Sleep(10000);
            var host = _mailServiceSettings.Host;
            var queues = _mailServiceSettings.Queues.GetAll();

            var workersCount = _mailServiceSettings.WorkersCount;
            _logger.Info(string.Format("Host: {0}, workers count: {1}.", host, workersCount));

            var factory = new ConnectionFactory
                                 {
                                     HostName = host,
                                     VirtualHost = _mailServiceSettings.VirtualHost,
                                     Protocol = Protocols.DefaultProtocol,
                                     Port = AmqpTcpEndpoint.UseDefaultPort,
                                 };

            if (_mailServiceSettings.Username != null && _mailServiceSettings.Password != null)
            {
                factory.UserName = _mailServiceSettings.Username;
                factory.Password = _mailServiceSettings.Password;
            }

            using (var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                foreach (var queue in queues)
                {
                    channel.QueueDeclare(queue.Name, true, false, false, null);
                }
            }

            for (var i = 0; i < workersCount; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var worker = new RabbitMqMailServerWorker();
                    _workers.Add(worker);
                    worker.DoWork(factory, queues);
                });
            }

            _logger.Info("Service started.");

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            _logger.Info("Service stopping...");
            var tasks = _workers.Select(mailServiceWorker => mailServiceWorker.Stop())
                                .ToList();
            _logger.Info("Wait for workers stops...");

            tasks.ForEach(x => x.Wait(new TimeSpan(0, 0, 30)));
            _logger.Info("Workers stops.");
            _logger.Info("Service stopped.");

            base.OnStop();
        }
    }
}
