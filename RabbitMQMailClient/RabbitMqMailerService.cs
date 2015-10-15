using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQMailClient.ConfigurationSections;
using RabbitMQMailClient.Interfaces;
using RabbitMQMailClient.Models;
using RabbitMQMailClient.TemplateBases;
using RabbitMQMailClient.TemplateResolvers;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace RabbitMQMailClient
{
    public class RabbitMqMailerService: IRabbitMqMailerService
    {
        private readonly RabbitMqMailClientSettings _rabbitMqMailClientSettings;
        private readonly SmtpClient _smtp;

        public RabbitMqMailerService()
        {
            _rabbitMqMailClientSettings = ConfigurationManager.GetSection("rabbitMqMailClient") as RabbitMqMailClientSettings;
            _smtp = new SmtpClient();

            var templateConfig = new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof (RabbitMQMailClientTemplateBase<>),
                Resolver = new RabbitMQMailClientTemplateResolver()
            };
            Razor.SetTemplateService(new TemplateService(templateConfig));
        }

        public MailModel GenerateModel(string subject, string receiver, string view)
        {
            return GenerateModel<object>(subject, receiver, view, null);
        }

        public void QueueMail<T>(string subject, IEnumerable<string> receivers, string view, T model)
        {
            var models = receivers.Select(x => GenerateModel(subject, x, view, model));
            QueueMail(models);
        }

        public void QueueMail(string subject, IEnumerable<string> receivers, string view)
        {
            QueueMail<object>(subject, receivers, view, null);
        }

        public void QueueMail<T>(string subject, string receiver, string view, T model)
        {
            QueueMail(subject, new [] { receiver }, view, model);
        }

        public void QueueMail(string subject, string receiver, string view)
        {
            QueueMail(subject, new [] { receiver }, view);
        }

        public void QueueMail(IEnumerable<MailModel> models)
        {
            var host = _rabbitMqMailClientSettings.Host;
            var queue = _rabbitMqMailClientSettings.Queue;

            var factory = new ConnectionFactory
            {
                HostName = host,
                VirtualHost = _rabbitMqMailClientSettings.VirtualHost,
                Protocol = Protocols.DefaultProtocol,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };

            if (_rabbitMqMailClientSettings.Username != null && _rabbitMqMailClientSettings.Password != null)
            {
                factory.UserName = _rabbitMqMailClientSettings.Username;
                factory.Password = _rabbitMqMailClientSettings.Password;
            }

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                foreach (var model in models)
                {
                    QueueMail(model, channel, queue);
                }
            }
        }

        public void QueueMail(MailModel model)
        {
            var host = _rabbitMqMailClientSettings.Host;
            var queue = _rabbitMqMailClientSettings.Queue;

            var factory = new ConnectionFactory
            {
                HostName = host,
                VirtualHost = _rabbitMqMailClientSettings.VirtualHost,
                Protocol = Protocols.DefaultProtocol,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };

            if (_rabbitMqMailClientSettings.Username != null && _rabbitMqMailClientSettings.Password != null)
            {
                factory.UserName = _rabbitMqMailClientSettings.Username;
                factory.Password = _rabbitMqMailClientSettings.Password;
            }

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                QueueMail(model, channel, queue);
            }
        }

        public void SendMessage(MailModel model)
        {
            var message = GenerateMessage(model);

            _smtp.Send(message);
        }

        private MailModel GenerateModel<T>(string subject, string receiver, string view, T model)
        {
            var fullViewPath = System.Web.Hosting.HostingEnvironment.MapPath(view);
            var viewContent = File.ReadAllText(fullViewPath);
            var body = Razor.Parse(viewContent, model);

            var result = new MailModel
            {
                Subject = subject,
                Receiver = receiver,
                Body = body
            };

            return result;
        }

        private void QueueMail(MailModel model, IModel channel, string queue)
        {
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.SetPersistent(true);
            var json = JsonConvert.SerializeObject(model,
                                                   new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});

            channel.BasicPublish("", queue, basicProperties, Encoding.UTF8.GetBytes(json));
        }

        private MailMessage GenerateMessage(MailModel model)
        {
            var message = new MailMessage();
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Subject = model.Subject;
            message.Body = model.Body;
            message.To.Add(model.Receiver);

            return message;
        }
    }
}