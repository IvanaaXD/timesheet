using AutoMapper;
using TimeSheet.Application.DTOs.Activity;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Application.Mappings
{
    public class ActivityMappingProfile : Profile
    {
        public ActivityMappingProfile()
        {
            CreateMap<Activity, ActivityDTO>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Project.Client.Name))
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name));

            CreateMap<ActivityRequestDTO, Activity>();
        }
    }
}