using ProductService.Models;

namespace ProductService.Service.IFolder
{
    public interface IProductService
    {
        Task<ResponseDto?> GetProductByNameAsync(string productName);
        Task<ResponseDto?> GetAllPorductsAsync(); //make return type non-list??
        Task<ResponseDto?> GetProductByIdAsync(int productId);
        Task<ResponseDto?> CreateProductAsync(ProductDto productDto);
        Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);
        Task<ResponseDto?> DeleteProductAsync(int productId);
    }
}
