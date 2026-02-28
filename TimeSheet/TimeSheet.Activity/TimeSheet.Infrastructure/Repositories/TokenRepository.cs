using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;
using TimeSheet.Domain.Common.Models;

namespace TimeSheet.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TimeSheetDbContext _context;

        public TokenRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> FindRefreshToken(Guid memberId)
        {
            return await _context.RefreshTokens
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x => x.Member.Id == memberId && !x.IsRevoked);
        }

        public async Task<RefreshToken?> FindRefreshTokenByTokenString(string tokenString)
        {
            return await _context.RefreshTokens
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x => x.TokenString == tokenString && !x.IsRevoked);
        }

        public async Task RevokeRefreshToken(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
        
        public async Task SaveRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
