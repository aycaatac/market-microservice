namespace OrderService.API.Service
{
    public interface IMessageProducer
    {
        public void SendMessage<T>(T message);
    }
}
