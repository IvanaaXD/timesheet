using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Auth;

namespace TimeSheet.Application.Abstractions
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> GenerateAccessTokenFromRefreshToken(string refreshToken, string accessToken);
        Task LogoutAsync(LogoutRequest request);
    }
}