using Mongo.Services.ShoppingCartAPI.Model.Dto;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mongo.Services.ShoppingCartAPI.Models.Dto
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
