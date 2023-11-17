using Mongo.Services.OrderAPI.Models.Dto;
using Newtonsoft.Json;

namespace Mongo.Services.ShoppingCartAPI.Models.Dto
{
    public class PayStackDto
    {
        [JsonProperty("reference")]
        public string? Reference {  get; set; }
        [JsonProperty("cancelUrl")]
        public string CancelUrl { get; set; }
        [JsonProperty("authorization")]
        public string? Authorization { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("orderHeader")]
        public OrderHeaderDto OrderHeader { get; set; }

    }
}
