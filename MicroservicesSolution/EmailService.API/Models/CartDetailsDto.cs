


namespace EmailService.API.Models
{
    public class CartDetailsDto
    {

        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }

       
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }

        public int ProductCount { get; set; }
    }
}
