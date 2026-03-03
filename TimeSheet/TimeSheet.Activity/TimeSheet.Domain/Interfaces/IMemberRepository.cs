using System;
using System.Threading.Tasks;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface IMemberRepository
    {
        Task<Member> FindMemberByIdAsync(Guid id);
        Task<Member> FindMemberByUsernameAsync(string username);
        Task<Member> FindMemberByEmailAsync(string email);
        Task<IEnumerable<Member>> FindAllMembersAsync();
        Task<Member> AddMemberAsync(Member member);
        Task<Member> UpdateMemberAsync(Member member);
        Task UpdateMemberPasswordAsync(Member member);
        Task DeleteMemberAsync(Member member);
        Task<PagedList<Member>> FindAllMembersPagedAsync(
           int pageNumber,
           int pageSize,
           string order);
    }
}
