using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.API.Models.DTO
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }     
        public double Discount { get; set; }        
        public double CartTotal { get; set; }
     
        public string? Name { get; set; }
   
        public string? PhoneNumber { get; set; }
    
        public string? Email { get; set; }
       
        public string? Address { get; set; }
    }
}
