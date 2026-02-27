using AutoMapper;
using TimeSheet.Application.DTOs.Project;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Common.Models;

namespace TimeSheet.Application.Mappings
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            CreateMap<ProjectRequestDTO, Project>()
            .ForMember(dest => dest.CurrentLeadId, opt => opt.MapFrom(src => src.CurrentLead))
            .ForMember(dest => dest.CurrentLead, opt => opt.Ignore());

            CreateMap<Project, ProjectDTO>()
            .ForMember(dest => dest.ClientName,
                       opt => opt.MapFrom(src => src.Client.Name))
            .ForMember(dest => dest.CurrentLeadName,
                       opt => opt.MapFrom(src => src.CurrentLead.Name));

            CreateMap(typeof(PagedList<>), typeof(PagedList<>));
        }
    }
}