using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQMailClient.Models;

namespace RabbitMQMailClient.Extensions
{
    public static class ObjectExtensions
    {
        public static void SendInRabbitMQ<T>(this T message, string host, string queue, string virtualHost, string username = null, string password = null)
        {
            var factory = new ConnectionFactory
                          {
                              HostName = host,
                              VirtualHost = virtualHost,
                              Protocol = Protocols.DefaultProtocol,
                              Port = AmqpTcpEndpoint.UseDefaultPort
                          };

            if (username != null && password != null)
            {
                factory.UserName = username;
                factory.Password = password;
            }

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.SetPersistent(true);
                var json = JsonConvert.SerializeObject(message, new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.All });
                channel.BasicPublish("", queue, basicProperties, Encoding.UTF8.GetBytes(json));
            }
        }

        public static byte[] ToByteArray<T>(this T obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ByteArrayToObject<T>(this byte[] array)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(array))
            {
                var result = bf.Deserialize(ms);

                return (T)result;
            }
        }
    }
}