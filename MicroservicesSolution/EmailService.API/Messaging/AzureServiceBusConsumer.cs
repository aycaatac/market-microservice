using Azure.Messaging.ServiceBus;
using EmailService.API.Models;
using Newtonsoft.Json;
using System.Text;

namespace EmailService.API.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration configuration;

        private ServiceBusProcessor emailCartProcessor;


        public AzureServiceBusConsumer(IConfiguration configuration)
        {
            this.configuration = configuration;
            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            emailCartProcessor = client.CreateProcessor(emailCartQueue);            
        }

        public async Task Start()
        {
            emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestRecieved;
            emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await emailCartProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await emailCartProcessor.StopProcessingAsync();
            await emailCartProcessor.DisposeAsync();
        }


        private async Task OnEmailCartRequestRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            ShoppingCartDto objMessage = JsonConvert.DeserializeObject<ShoppingCartDto>(body);

            try
            {
                //try to log email
                await args.CompleteMessageAsync(args.Message);
            }catch(Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            //send email for error here
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        
    }
}
