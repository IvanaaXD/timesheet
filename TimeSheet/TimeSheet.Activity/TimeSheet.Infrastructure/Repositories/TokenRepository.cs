using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;

namespace TimeSheet.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TimeSheetDbContext _context;

        public TokenRepository(TimeSheetDbContext context)
        {
            _context = context;
        }

        public Task<RefreshToken> FindRefreshToken(Guid memberId)
        {

        }

        public Task<RefreshToken> GenerateRefreshToken()
        {

        }

        public Task RevokeRefreshToken(RefreshToken refreshToken)
        {

        }
        
        public Task SaveRefreshToken(Guid memberId, RefreshToken refreshToken)
        {

        }
    }
}
