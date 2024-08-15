using ShoppingCartService.API.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;

namespace ShoppingCartService.API.Models.Domain
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        [ForeignKey("CartHeaderId")]
        public CartHeader CartHeader { get; set; }
        public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        //[NotMapped]
        //public ProductDto productDto { get; set; }
        [Range(0,1000)]
        public int ProductCount { get; set; } //bir urunden kac adet var
    }
}
