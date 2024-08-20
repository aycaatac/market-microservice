using ProductService.Models;

namespace ProductService.Service.IFolder
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(ShoppingCartDto cartDto);        
    }
}
