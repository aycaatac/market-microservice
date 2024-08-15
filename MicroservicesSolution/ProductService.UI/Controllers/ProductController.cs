using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service.IFolder;
using System.Reflection;

namespace ProductService.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> products = new();
            
            ResponseDto response = await productService.GetAllPorductsAsync();

            if(response != null && response.IsSuccess == true)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        public async Task<IActionResult> ProductCreate()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await productService.CreateProductAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int productId)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await productService.DeleteProductAsync(productId);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View();
        }

        public async Task<IActionResult> ProductUpdate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await productService.UpdateProductAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }
    }
}
