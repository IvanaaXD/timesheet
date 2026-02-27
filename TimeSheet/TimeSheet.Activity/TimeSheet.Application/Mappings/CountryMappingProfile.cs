using AutoMapper;
using TimeSheet.Application.DTOs.Country;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Application.Mappings
{
    public class CountryMappingProfile : Profile
    {
        public CountryMappingProfile()
        {
            CreateMap<Country, CountryDTO>();
            CreateMap<CountryRequestDTO, Country>();
        }
    }
}