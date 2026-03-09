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
            .ForMember(dest => dest.CurrentLeadId, opt => opt.MapFrom(src => src.CurrentLeadId))
            .ForMember(dest => dest.TeamMembers, opt => opt.Ignore());

            CreateMap<Project, ProjectDTO>()
            .ForMember(dest => dest.ClientName,
                        opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : "No Client"))
            .ForMember(dest => dest.CurrentLeadName,
                        opt => opt.MapFrom(src => src.CurrentLead != null ? src.CurrentLead.Name : "No Lead"))
            .ForMember(dest => dest.TeamMembers,
                        opt => opt.MapFrom(src => src.TeamMembers.Select(tm => tm.Member.Name)));

            CreateMap(typeof(PagedList<>), typeof(PagedList<>));
        }
    }
}