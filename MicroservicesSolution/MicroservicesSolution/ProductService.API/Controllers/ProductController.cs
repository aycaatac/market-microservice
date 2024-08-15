using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Data;
using ProductService.API.Models.Domain;
using ProductService.API.Models.DTO;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppProductDbContext appProductDbContext;
        private readonly IMapper mapper;

        public ProductController(AppProductDbContext appProductDbContext, IMapper mapper)
        {
            this.appProductDbContext = appProductDbContext;
            this.mapper = mapper;
        }

        [HttpGet]
       // [Authorize(Roles = "admın")]
        [Route("GetAll")]
        public ResponseDto GetAll()
        {
            ResponseDto resp = new();
            IEnumerable<Product> products = appProductDbContext.Products.ToList<Product>(); //?
            resp.IsSuccess = true;
            resp.Result = mapper.Map<List<ProductDto>>(products);
            return resp;
        }

        [HttpGet]       
     
        [Route("GetAllResponse")]
        public ResponseDto GetAllResponse()
        {
            IEnumerable<Product> products = appProductDbContext.Products.ToList<Product>(); //?
            ResponseDto resp = new()
            {
                IsSuccess = true,
                Result = mapper.Map<List<ProductDto>>(products)
            };
            return resp;
        }

        [HttpGet]
        //[Authorize]
        [Route("GetById/{id:int}")]
        public ResponseDto GetById([FromRoute] int id)
        {
            ResponseDto resp = new();
            Product product = appProductDbContext.Products.FirstOrDefault(x => x.ProductId == id);
            if(product == null)
            {
                resp.IsSuccess = false;
                resp.Message = "Not Found";
                return resp;
            }
            resp.IsSuccess = true;
            resp.Result = mapper.Map<ProductDto>(product);
            return resp;
        }


        [HttpGet]
        //[Authorize]
        [Route("GetByName/{name}")]
        public ResponseDto GetByName([FromRoute] string name)
        {
            ResponseDto resp = new();
            Product product = appProductDbContext.Products.FirstOrDefault(x => x.Name == name);
            if (product == null)
            {
                resp.IsSuccess = false;
                resp.Message = "Not Found";
                return resp;
            }
            resp.IsSuccess = true;
            resp.Result = mapper.Map<ProductDto>(product);
            return resp;
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        [Route("Create")]
        public ResponseDto Create([FromBody] InputProductDto productDto)
        {
            ResponseDto resp = new();
            var productDomainModel = mapper.Map<Product>(productDto);
            appProductDbContext.Products.Add(productDomainModel);
            appProductDbContext.SaveChanges();

            resp.IsSuccess = true;
            resp.Result = mapper.Map<ProductDto>(productDomainModel);
            return resp;

        }

        [HttpPut]
        //[Authorize(Roles = "admin")]
        [Route("Update/{id:int}")]
        public ResponseDto Update([FromRoute] int id, [FromBody] InputProductDto productDto)
        {
            ResponseDto resp = new();
            var productDomainModel = appProductDbContext.Products.FirstOrDefault(x => x.ProductId == id);
            if(productDomainModel == null)
            {
                resp.IsSuccess = false;
                resp.Message = "Not Found";
                return resp;
            }
            productDomainModel.Name = productDto.Name;
            productDomainModel.Price = productDto.Price;
            productDomainModel.Description = productDto.Description;
            productDomainModel.CategoryName = productDto.CategoryName;
            productDomainModel.ImageUrl = productDto.ImageUrl;
            appProductDbContext.SaveChanges();
           
            resp.IsSuccess = true;
            resp.Result = mapper.Map<ProductDto>(productDomainModel);
            return resp;
        }

        [HttpDelete]
        //[Authorize(Roles = "admin")]
        [Route("Delete/{id:int}")]
        public ResponseDto Delete([FromRoute] int id)
        {
            ResponseDto resp = new();
            var productDomainModel = appProductDbContext.Products.FirstOrDefault(x => x.ProductId == id);
            if (productDomainModel == null)
            {
                resp.IsSuccess = false;
                resp.Message = "Not Found";
                return resp;
            }
            appProductDbContext.Products.Remove(productDomainModel);
            appProductDbContext.SaveChanges();
            resp.IsSuccess = true;
            resp.Result = mapper.Map<ProductDto>(productDomainModel);
            return resp;
        }
    }
}
