using Azure.Messaging.ServiceBus;
using MailService.API.Models;
using MailService.API.Services;
using Newtonsoft.Json;
using System.Text;

namespace MailService.API.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly IConfiguration configuration;
        private readonly MailServiceImp mailService;
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;

        private ServiceBusProcessor emailCartProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, MailServiceImp mailService)
        {
            this.configuration = configuration;
            this.mailService = mailService;
            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:AycaMarketEmailQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            emailCartProcessor = client.CreateProcessor(emailCartQueue);

           

        }

        public async Task Start()
        {
            emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await emailCartProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {
            await emailCartProcessor.StopProcessingAsync();
            await emailCartProcessor.DisposeAsync();
        }


        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            //SEND MAIL RATHER THAN WRITING IN CONSOLE HERE
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            ShoppingCartDto objMessage = JsonConvert.DeserializeObject<ShoppingCartDto>(body);

            try
            {
                //TRY TO LOG MAIL
               await  mailService.MailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }catch(Exception ex)
            {
                //MANAGE EXCEPTION
                Console.WriteLine(ex.Message.ToString());
                throw;
            }
        }

        
    }
}
