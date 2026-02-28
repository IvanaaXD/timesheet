using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Domain.Common.Models;

namespace TimeSheet.Domain.Interfaces
{
    public interface ITokenRepository
    {
        Task<RefreshToken> FindRefreshToken(Guid memberId);
        Task<RefreshToken> FindRefreshTokenByTokenString(string tokenString);
        Task RevokeRefreshToken(RefreshToken refreshToken);
        Task SaveRefreshToken(RefreshToken refreshToken);
    }
}