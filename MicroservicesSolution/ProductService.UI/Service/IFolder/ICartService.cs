using Microsoft.AspNetCore.Mvc;
using ProductService.Models;

namespace ProductService.Service.IFolder
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartAsync(string userId);
        Task<ResponseDto?> ApplyCoupon(string couponCode, int cartDetailsID);
        Task<ResponseDto?> RemoveCoupon(string couponCode, int cartDetailsID);
        Task<ResponseDto?> Upsert(ShoppingCartDto shoppingCartDto);
        Task<ResponseDto?> DeleteOneKindOfProduct(int cartDetailsId);
        Task<ResponseDto?> DeleteOneProduct(int cartDetailsId);
    }
}
