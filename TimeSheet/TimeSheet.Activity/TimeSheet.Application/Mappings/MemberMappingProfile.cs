using AutoMapper;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Application.Mappings
{
    public class MemberMappingProfile : Profile
    {
        public MemberMappingProfile()
        {
            CreateMap<Member, MemberDTO>()
            .ForMember(dest => dest.ProjectNames,
                        opt => opt.MapFrom(src => src.ProjectMemberships.Select(pm => pm.Project.Name)));

            CreateMap<MemberRequestDTO, Member>();
        }
    }
}