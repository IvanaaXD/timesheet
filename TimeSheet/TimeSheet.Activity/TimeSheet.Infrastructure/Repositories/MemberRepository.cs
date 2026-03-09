using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;
using TimeSheet.Domain.Common.Models; 
using TimeSheet.Domain.Entities;     

namespace TimeSheet.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly TimeSheetDbContext _context;

        public MemberRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        private IQueryable<Member> GetMembersWithIncludes()
        {
            return _context.Members
                .Include(m => m.ProjectMemberships)
                    .ThenInclude(pm => pm.Project) 
                .AsQueryable();
        }

        public async Task<Member> FindMemberByIdAsync(Guid id)
        {
            return await GetMembersWithIncludes().FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Member> FindMemberByUsernameAsync(string username)
        {
            return await GetMembersWithIncludes().FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<Member> FindMemberByEmailAsync(string email)
        {
            return await GetMembersWithIncludes().FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<IEnumerable<Member>> FindAllMembersAsync()
        {
            return await GetMembersWithIncludes().AsNoTracking().ToListAsync();
        }

        public async Task<Member> AddMemberAsync(Member member)
        {
            _context.Members.Add(member);   
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<Member> UpdateMemberAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
            return member;
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

        public async Task<PagedList<Member>> FindAllMembersPagedAsync(
            int pageNumber,
            int pageSize,
            string order)
        {
            var query = GetMembersWithIncludes().AsNoTracking();

            query = order.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedList<Member>(items, totalCount, pageNumber, pageSize, null);
        }
    }
}
