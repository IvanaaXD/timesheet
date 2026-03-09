using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Member;
using TimeSheet.Application.Common.DTOs;
using TimeSheet.Domain.Common.Models; 
using TimeSheet.Domain.Entities;     

namespace TimeSheet.Application.Abstractions
{
    public interface IMemberService
    {
        Task<MemberDTO> GetMemberByIdAsync(Guid id);
        Task<MemberDTO> GetMemberByUsernameAsync(string username);
        Task<MemberDTO> GetMemberByEmailAsync(string email);
        Task<IEnumerable<MemberDTO>> GetAllMembersAsync();
        Task<PagedList<MemberDTO>> GetAllMembersPagedAsync(PagedListDTO pagedListDTO);
        Task<MemberDTO> CreateMemberAsync(MemberRequestDTO request);
        Task<MemberDTO> UpdateMemberAsync(Guid id, MemberRequestDTO request);
        Task<MemberDTO> UpdateMemberPasswordAsync(Guid id);
        Task DeleteMemberAsync(Guid id);
    }
}
