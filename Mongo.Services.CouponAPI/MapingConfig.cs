using AutoMapper;
using Mongo.Services.CouponAPI.Model;
using Mongo.Services.CouponAPI.Model.Dto;

namespace Mongo.Services.CouponAPI
{
    public class MapingConfig
    {
        public static MapperConfiguration ResisterMaps ()
        {
            var mapingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();

            });

            return mapingConfig;
        }
    }
}
