using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderService.API.Models.DTO;
using OrderService.API.Service.IService;
using System.Text.Json.Serialization;



namespace OrderService.API.Service
{
    public class ProductServiceImp : IProductService
    {
        private readonly IHttpClientFactory clientFactory;

        public ProductServiceImp(IHttpClientFactory clientFactory)
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
