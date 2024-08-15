using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.API.Models.DTO;

namespace ShoppingCartService.API.Service.IService
{
    //repsonsible to load the product from product api
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
