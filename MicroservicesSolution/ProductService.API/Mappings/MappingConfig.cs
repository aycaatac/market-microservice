using AutoMapper;
using ProductService.API.Models.Domain;
using ProductService.API.Models.DTO;


namespace ProductService.API.Mappings
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
                config.CreateMap<InputProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
