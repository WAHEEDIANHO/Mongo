using System.ComponentModel.DataAnnotations;

namespace Mongo.Services.TrierAPI.Model
{
    public class Trier
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string title {  get; set; }
        [Required]
        public string description { get; set; }
    }
}
