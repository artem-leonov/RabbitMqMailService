using System.Collections.Generic;
using RabbitMQMailClient.Models;

namespace RabbitMQMailClient.Interfaces
{
    public interface IRabbitMqMailerService
    {
        MailModel GenerateModel(string subject, string receiver, string view);

        void QueueMail<T>(string subject, IEnumerable<string> receivers, string view, T model);

        void QueueMail(string subject, IEnumerable<string> receivers, string view);

        void QueueMail<T>(string subject, string receiver, string view, T model);

        void QueueMail(string subject, string receiver, string view);

        void QueueMail(IEnumerable<MailModel> models);

        void QueueMail(MailModel model);

        void SendMessage(MailModel model);
    }
}