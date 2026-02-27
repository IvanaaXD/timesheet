using AutoMapper;
using TimeSheet.Application.DTOs.Client;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Common.Models;

namespace TimeSheet.Application.Mappings
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile()
        {
            CreateMap<ClientRequestDTO, Client>();

            CreateMap<Client, ClientDTO>()
            .ForMember(dest => dest.CountryName,
                       opt => opt.MapFrom(src => src.Country.Name));

            CreateMap(typeof(PagedList<>), typeof(PagedList<>));
        }
    }
}