using System.ComponentModel.DataAnnotations;

namespace OrderService.API.Models.Domain
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public double OrderTotal { get; set; }
        public DateTime CreationDate { get; set; }
        public int TotalItemCount { get; set; }
    }
}
