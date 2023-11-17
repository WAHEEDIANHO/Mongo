using AutoMapper;
using Mongo.Services.OrderAPI.Models;
using Mongo.Services.OrderAPI.Models.Dto;

namespace Mongo.Services.ShoppingCartAPI
{
    public class MapingConfig
    {
        public static MapperConfiguration ResisterMaps ()
        {
            var mapingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.cartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();
                config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.product.Price));
                
                config.CreateMap<OrderDetailsDto, CartDetailsDto>().ReverseMap();

                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetailsDto, OrdertDetails>().ReverseMap();

            });

            return mapingConfig;
        }
    }
}
