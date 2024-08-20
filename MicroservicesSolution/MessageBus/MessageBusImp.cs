using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageBus
{
    public class MessageBusImp : IMessageBus
    {
        private string connectionString = "Endpoint=sb://aycamarket.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ld6laE10rGTIq6s76qZCp+pt9CDNe8CRO+ASbIzatBk=";
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            try
            {
                await using var client = new ServiceBusClient(connectionString);

                ServiceBusSender sender = client.CreateSender(topic_queue_name);

                var jsonMessage = JsonConvert.SerializeObject(message);

                ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
                {
                    CorrelationId = Guid.NewGuid().ToString(),
                };

                await sender.SendMessageAsync(finalMessage);
                await client.DisposeAsync();
            }catch(Exception ex)
            {
                string s = ex.Message.ToString();
                throw;
            }
        }
    }
}
