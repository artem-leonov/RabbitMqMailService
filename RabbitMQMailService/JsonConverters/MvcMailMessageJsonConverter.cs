using System;
using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RabbitMQMailServer.JsonConverters
{
    public class MvcMailMessageJsonConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);
            var address = jsonObject["Address"].Value<string>();
            var mailAddress = new MailAddress(address);

            return mailAddress;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (MailAddress);
        }
    }
}