using Mongo.Services.CouponAPI.Model.Dto;
using Mongo.Services.ShoppingCartAPI.Model.Dto;
using Mongo.Services.ShoppingCartAPI.Services.Iservice;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mongo.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory) { 
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDto> GetCoupon(string code)
        {
            HttpClient client = _httpClientFactory.CreateClient("CouponAPI");
            var response = await client.GetAsync($"/api/GetByCode/{code}");
            var apicontent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apicontent);
            if (resp.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }
            return new CouponDto();
        }
    }
}
