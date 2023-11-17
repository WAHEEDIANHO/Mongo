using Mongo.Services.OrderAPI.Model.Dto;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mongo.Services.OrderAPI.Models.Dto
{
    public class CartDetailsDto
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDto? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDto? product { get; set; }
        public int Count { get; set; }
    }
}
