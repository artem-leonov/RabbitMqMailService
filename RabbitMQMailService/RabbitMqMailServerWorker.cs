using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQMailClient;
using RabbitMQMailClient.Models;
using RabbitMQMailServer.ConfigurationSections;
using RabbitMQMailServer.Interfaces;

namespace RabbitMQMailServer
{
    public class RabbitMqMailServerWorker: IMailServiceWorker
    {
        private readonly Logger _logger;
        private readonly RabbitMqMailerService _mailer;

        private bool _stopping;
        private bool _stopped;

        public RabbitMqMailServerWorker()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _mailer = new RabbitMqMailerService();
        }

        public void DoWork(ConnectionFactory factory, IEnumerable<RabbitMqMailServerQueuesConfigurationElement> queues)
        {
            _logger.Info("Worker initialization...");

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                List<QueueingBasicConsumer> consumers = new List<QueueingBasicConsumer>();

                foreach (var queue in queues.OrderByDescending(x => x.Priority))
                {
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queue.Name, false, consumer);
                    consumers.Add(consumer);
                }

                _logger.Info("Worker initialized.");

                while (!_stopping)
                {
                    BasicDeliverEventArgs eventArgs = null;

                    foreach (QueueingBasicConsumer consumer in consumers)
                    {
                        eventArgs = consumer.Queue.DequeueNoWait(null);

                        if(eventArgs != null)
                            break;
                    }

                    if (eventArgs != null)
                    {
                        _logger.Info("Message preparation...");
                        var body = Encoding.UTF8.GetString(eventArgs.Body);

                        MailModel message = null;
                        try
                        {
                            message = JsonConvert.DeserializeObject<MailModel>(body, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("Ошибка конвертации сообщения.", ex);
                            throw;
                        }

                        _logger.Info("Message prepared.");
                        try
                        {
                            _mailer.SendMessage(message);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("Ошибка отправки письма", ex);
                            throw;
                        }
                        _logger.Info("Message sended.");
                        channel.BasicAck(eventArgs.DeliveryTag, false);
                        _logger.Info("Channel acknowledged.");
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            
            _stopped = true;
        }

        public async Task Stop()
        {
            await Task.Factory.StartNew(() =>
            {
                _logger.Info("Worker stopping...");
                _stopping = true;

                while (!_stopped)
                {
                    Thread.Sleep(100);
                }

                _logger.Info("Worker stopped.");
            });
        }
    }
}