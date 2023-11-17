using Mongo.Services.CouponAPI.Model.Dto;
using Mongo.Services.ShoppingCartAPI.Model.Dto;

namespace Mongo.Services.ShoppingCartAPI.Services.Iservice
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string code);
    }
}
