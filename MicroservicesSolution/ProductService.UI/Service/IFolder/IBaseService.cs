using ProductService.Models;

namespace ProductService.Service.IFolder
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto);       
    }
}
