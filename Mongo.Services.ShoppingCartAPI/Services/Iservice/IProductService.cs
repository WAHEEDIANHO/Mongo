using Mongo.Services.ShoppingCartAPI.Model.Dto;

namespace Mongo.Services.ShoppingCartAPI.Services.Iservice
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
