using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service.IFolder;
using ProductService.Utility;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace ProductService.Controllers
{
    public class OrderController : Controller
    {
		private readonly IOrderService orderService;

		public OrderController(IOrderService orderService)
        {
			this.orderService = orderService;
		}

        public IActionResult OrderIndex()
        {
            return View();
        }

		public async Task<IActionResult> OrderDetail(int orderId)
		{
            TempData["success"] = null;
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
			string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await orderService.GetOrder(orderId);
			if (response.IsSuccess)
			{
				orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
			}
			if(!User.IsInRole(SD.RoleAdmin.ToLower()) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
		}

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            TempData["success"] = null;
            var response = await orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if (response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
           
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            TempData["success"] = null;
            var response = await orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            TempData["success"] = null;
            var response = await orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetAll(string? status)
        {
            IEnumerable<OrderHeaderDto> list;
            string userId = "";
            if (!User.IsInRole(SD.RoleAdmin.ToLower()))
            {
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            }
            ResponseDto response = orderService.GetAllOrder(userId).GetAwaiter().GetResult();
            if (response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));
                if (!string.IsNullOrEmpty(status))
                {
                    switch (status)
                    {
                        case "approved":
                            list = list.Where(x => x.Status == SD.Status_Approved);
                            break;
                        case "readyforpickup":
                            list = list.Where(x => x.Status == SD.Status_ReadyForPickup);
                            break;
                        case "cancelled":
                            list = list.Where(x => x.Status == SD.Status_Cancelled);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                list = new List<OrderHeaderDto>();
            }
            return Json(new { data = list });
        }
    }
}
