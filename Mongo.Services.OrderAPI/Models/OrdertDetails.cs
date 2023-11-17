using Mongo.Services.OrderAPI.Model.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mongo.Services.OrderAPI.Models
{
    public class OrdertDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader {  get; set; } 
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}
