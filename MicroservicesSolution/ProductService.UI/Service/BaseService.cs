using Newtonsoft.Json;
using ProductService.Models;
using ProductService.Service.IFolder;
using System.Net;
using System.Text;
using static ProductService.Utility.SD;

namespace ProductService.Service
{
    public class BaseService : IBaseService
    {
        
        private readonly IHttpClientFactory clientFactory;
		private readonly ITokenProvider tokenProvider;

		public BaseService(IHttpClientFactory clientFactory, ITokenProvider tokenProvider)
        {
            this.clientFactory = clientFactory;
			this.tokenProvider = tokenProvider;
		}

        //access token????????
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {
                HttpClient client = clientFactory.CreateClient("ProductApi");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                //add token
                message.RequestUri = new Uri(requestDto.Url);
				
					var token = tokenProvider.GetToken();
					message.Headers.Add("Authorization", $"Bearer {token}");
				
				if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                switch (requestDto.ApiType)
                {
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;

                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete
                            ;
                        break;
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;

                }
                
                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new()
                        {
                            IsSuccess = false, Message = "Not Found"
                        };

                    case HttpStatusCode.Forbidden:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "Forbidden"
                        };

                    case HttpStatusCode.Unauthorized:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "Unauthorized"
                        };

                    case HttpStatusCode.InternalServerError:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "InternalServerError"
                        };
                    default:
                        var content = await apiResponse.Content.ReadAsStringAsync();
                        var respDto = JsonConvert.DeserializeObject<ResponseDto>(content);                       
                        return respDto;
                }
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false, Message = ex.Message
                };
            }
        }
    }
}
