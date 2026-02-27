using AutoMapper;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Application.Mappings
{
    public class MemberMappingProfile : Profile
    {
        public MemberMappingProfile()
        {
            CreateMap<Member, MemberDTO>();
            CreateMap<MemberRequestDTO, Member>();
        }
    }
}