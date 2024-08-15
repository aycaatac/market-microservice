using AutoMapper;
using OrderService.API.Models.Domain;
using OrderService.API.Models.DTO;


namespace OrderService.API.Mappings
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderDto,Order>().ReverseMap();
                
            });
            return mappingConfig;
        }
    }
}
