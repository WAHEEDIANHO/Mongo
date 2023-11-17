using Mongo.Services.OrderAPI.Model.Dto;

namespace Mongo.Services.OrderAPI.Services.Iservice
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
