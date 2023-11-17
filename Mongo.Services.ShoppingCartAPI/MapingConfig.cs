using AutoMapper;
using Mongo.Services.ShoppingCartAPI.Model;
using Mongo.Services.ShoppingCartAPI.Model.Dto;
using Mongo.Services.ShoppingCartAPI.Models;
using Mongo.Services.ShoppingCartAPI.Models.Dto;

namespace Mongo.Services.ShoppingCartAPI
{
    public class MapingConfig
    {
        public static MapperConfiguration ResisterMaps ()
        {
            var mapingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();

            });

            return mapingConfig;
        }
    }
}
