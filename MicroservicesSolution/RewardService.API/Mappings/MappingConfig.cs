using AutoMapper;
using RewardService.API.Models;


namespace RewardService.API.Mappings
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<RewardDto, Reward>().ReverseMap();                
            });
            return mappingConfig;
        }
    }
}
