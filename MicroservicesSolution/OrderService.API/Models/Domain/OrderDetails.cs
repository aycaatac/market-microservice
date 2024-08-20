using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderService.API.Models.DTO;

namespace OrderService.API.Models.Domain
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader { get; set; }

        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }

        public int ProductCount { get; set; }
        public string productName { get; set; }
        public double Price { get; set; } //if the price changes it stays the same at order
    }
}
