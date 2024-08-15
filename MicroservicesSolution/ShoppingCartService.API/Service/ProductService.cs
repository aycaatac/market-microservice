using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingCartService.API.Models.DTO;
using ShoppingCartService.API.Service.IService;
using ShoppingCartService.Models.DTO;
using System.Text.Json.Serialization;



namespace ShoppingCartService.API.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory clientFactory;

        public ProductService(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = clientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product/getall");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDto>();
        }
    }
}
