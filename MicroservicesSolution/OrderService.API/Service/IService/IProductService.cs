using Microsoft.AspNetCore.Mvc;
using OrderService.API.Models.DTO;

namespace OrderService.API.Service.IService
{
    //repsonsible to load the product from product api
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
