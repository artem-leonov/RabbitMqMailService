using System.Collections.Generic;

namespace RabbitMQMailClient.Models
{
    public class MailModel
    {
        public string Body { get; set; }

        public string Subject { get; set; }

        public string Receiver { get; set; }
    }
}