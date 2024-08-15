namespace OrderService.API.Models.DTO
{
    public class OrderDto
    {
        //should i keep the order id??
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public double OrderTotal { get; set; }
        public DateTime CreationDate { get; set; }
        public int TotalItemCount { get; set; }
    }
}
