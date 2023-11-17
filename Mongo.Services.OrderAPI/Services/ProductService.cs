using Mongo.Services.OrderAPI.Model.Dto;
using Mongo.Services.OrderAPI.Services.Iservice;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mongo.Services.OrderAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory) { 
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            HttpClient client = _httpClientFactory.CreateClient("ProductAPI");
            var response = await client.GetAsync($"/api/products");
            var apicontent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apicontent);

            if (resp.IsSuccessful)
            {
               return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            else { return new List<ProductDto>(); }
        }
    }
}
