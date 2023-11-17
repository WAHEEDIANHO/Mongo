using AutoMapper;
using Mongo.Services.ProductAPI.Model;
using Mongo.Services.ProductAPI.Model.Dto;

namespace Mongo.Services.ProductAPI
{
    public class MapingConfig
    {
        public static MapperConfiguration ResistMaps ()
        {
            var mapingconfig = new MapperConfiguration (config =>
            {
                config.CreateMap<ProductDto, Product>();
                config.CreateMap<Product, ProductDto>();
            });

            return mapingconfig;    
        }
    }
}
