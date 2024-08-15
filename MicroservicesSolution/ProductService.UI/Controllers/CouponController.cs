using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service.IFolder;

namespace ProductService.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService couponService;

        public CouponController(ICouponService couponService)
        {
            this.couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> coupons = new();

            ResponseDto response = await couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess == true)
            {
                coupons = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            return View(coupons);          
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await couponService.CreateCouponAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await couponService.DeleteCouponAsync(couponId);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View();
        }

        public async Task<IActionResult> CouponUpdate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await couponService.UpdateCouponAsync(model);

                if (response.IsSuccess == true) //questionable
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(model);
        }
    }
}

