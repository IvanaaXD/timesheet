using System;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Interfaces
{
    public interface IMemberRepository
    {
        Task<Member> FindMemberByIdAsync(Guid id);
        Task<Member> FindMemberByUsernameAsync(string username);
        Task<Member> FindMemberByEmailAsync(string email);
        Task<IEnumerable<Member>> FindAllMembersAsync();
        Task AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
        Task UpdateMemberPasswordAsync(Member member);
        Task DeleteMemberAsync(Member member);
        // paginacija
    }
}
