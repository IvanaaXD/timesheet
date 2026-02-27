using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly TimeSheetDbContext _context;

        public MemberRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public async Task<Member> FindMemberByIdAsync(Guid id)
        {
            return await _context.Members.FindAsync(id);
        }

        public async Task<Member> FindMemberByUsernameAsync(string username)
        {
            return await _context.Members.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<Member> FindMemberByEmailAsync(string email)
        {
            return await _context.Members.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<IEnumerable<Member>> FindAllMembersAsync()
        {
            return await _context.Members.AsNoTracking().ToListAsync();
        }

        public async Task AddMemberAsync(Member member)
        {
            _context.Members.Add(member);   
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMemberAsync(Member member)
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMemberPasswordAsync(Member member)
        {
            var existingMember = await _context.Members.FirstOrDefaultAsync(x => x.Username == member.Username);
            if (existingMember != null)
            {
                existingMember.Password = member.Password;

                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMemberAsync(Member member)
        {
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}
