using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;

namespace ProductService.Service
{
    public class ProductServiceImp : IProductService
    {
        private readonly IBaseService baseService;

        public ProductServiceImp(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                Url = SD.ProductApiBase + "/api/product/create"
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int productId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductApiBase + "/api/product/delete/" + productId
            });
        }

        public async Task<ResponseDto?> GetAllPorductsAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase+"/api/product/getallresponse"
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int productId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase + "/api/product/getbyid/"+productId
            });
        }

        public async Task<ResponseDto?> GetProductByNameAsync(string productName)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase + "/api/product/getbyname/" + productName
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                Url = SD.ProductApiBase + "/api/product/update/" + productDto.ProductId
            });
        }
    }
}
