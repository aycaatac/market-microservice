using Azure.Messaging.ServiceBus;

using Newtonsoft.Json;
using RewardService.API.Message;
using RewardService.API.Migrations;
using RewardService.API.Services;
using System.Text;

namespace RewardService.API.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly IConfiguration configuration;
        private readonly RewardServiceImp rewardService;
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardSubscription;

        private ServiceBusProcessor rewardProcessor;
       

        public AzureServiceBusConsumer(IConfiguration configuration, RewardServiceImp rewardService)
        {
            this.configuration = configuration;
            this.rewardService = rewardService;
            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");

            orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardSubscription = configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Reward_Subscription");
            var client = new ServiceBusClient(serviceBusConnectionString);

            rewardProcessor = client.CreateProcessor(orderCreatedTopic,orderCreatedRewardSubscription);            
        }

        public async Task Start()
        {
            rewardProcessor.ProcessMessageAsync += OnRewardRequestReceived;
            rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await rewardProcessor.StartProcessingAsync();

       
        }

        private async Task OnRewardRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardMessage objMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

            try
            {
             
                await rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                //MANAGE EXCEPTION
                Console.WriteLine(ex.Message.ToString());
                throw;
            }
        }

        public async Task Stop()
        {
            await rewardProcessor.StopProcessingAsync();
            await rewardProcessor.DisposeAsync();

           
        }


        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            //SEND MAIL RATHER THAN WRITING IN CONSOLE HERE
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
             

    }
}
