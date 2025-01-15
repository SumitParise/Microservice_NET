using CouponApi.Models.Dto;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using WebApp.Models;
using WebApp.Service.IService;
using static WebApp.Utility.SD;

namespace WebApp.Service
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public BaseService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		public async Task<responseCouponDto?> SendAsync(RequestCouponDto reqDto)
		{
			try
			{
				HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
				HttpRequestMessage message = new();
				message.Headers.Add("Accept", "application/json");

				message.RequestUri = new Uri(reqDto.Url);

				if (reqDto.Data != null)
				{
					message.Content = new StringContent(JsonConvert.SerializeObject(reqDto.Data), Encoding.UTF8, "application/json");
				}

				HttpResponseMessage? apiResponse = null;

				switch (reqDto.ApiType)
				{
					case ApiType.POST:
						message.Method = HttpMethod.Post;
						break;
					case ApiType.PUT:
						message.Method = HttpMethod.Put;
						break;
					case ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}

				apiResponse = await client.SendAsync(message);

				switch (apiResponse.StatusCode)
				{
					case System.Net.HttpStatusCode.NotFound:
						return new() { isSuccess = false, message = "Not Found" };

					case System.Net.HttpStatusCode.Forbidden:
						return new() { isSuccess = false, message = "Access Denied" };

					case System.Net.HttpStatusCode.Unauthorized:
						return new() { isSuccess = false, message = "Unauthorized" };

					case System.Net.HttpStatusCode.InternalServerError:
						return new() { isSuccess = false, message = "Internal Server Error" };

					default:
						var apiContent = await apiResponse.Content.ReadAsStringAsync();
						var apiResponseDto = JsonConvert.DeserializeObject<responseCouponDto>(apiContent);
						return apiResponseDto;
						break;
				}
			}
			catch (Exception ex)
			{
				var dto = new responseCouponDto()
				{
					message = ex.Message,
					isSuccess = false,
				};
				return dto;
			}
	    }
	}
}
