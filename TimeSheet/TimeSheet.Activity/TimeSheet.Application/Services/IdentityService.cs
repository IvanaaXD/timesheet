using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;

using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeSheet.Application.DTOs.Auth;
using TimeSheet.Application.Abstractions;
using System.Security.Cryptography;
using TimeSheet.Domain.Entities.Enums;
using TimeSheet.Domain.Common.Models;
using TimeSheet.Application.Common.Exceptions;

namespace TimeSheet.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IConfiguration _configuration;

        public IdentityService(IMemberRepository memberRepository, ITokenRepository tokenRepository, IConfiguration configuration)
        {
            _memberRepository = memberRepository;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var member = await _memberRepository.FindMemberByUsernameAsync(request.Username);
            if (member == null) throw new NotFoundException($"Member with username {request.Username} not found.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, member.Password);
            if (!isPasswordValid) throw new ValidationException("Incorrect credentials.");

            var accessToken = GenerateAccessToken(member.Id, member.Username, member.Role);

            var refreshToken = new RefreshToken
            {
                TokenString = GenerateRefreshToken(),
                MemberId = member.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _tokenRepository.SaveRefreshToken(refreshToken);

            return  new AuthResponse
            {
                Id = member.Id,
                Username = member.Username,
                AccessToken = accessToken,
                RefreshToken = refreshToken.TokenString
            };
        }

        public async Task LogoutAsync(LogoutRequest request)
        {
            var refreshToken = await _tokenRepository.FindRefreshTokenByTokenString(request.TokenString);

            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            await _tokenRepository.RevokeRefreshToken(refreshToken);
        }

        private string GenerateAccessToken(Guid id, string username, MemberRole role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role.ToString())
            }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateAccessTokenFromRefreshToken(string refreshToken,  string accessToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var username = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            var memberGuid = Guid.Parse(userId!);
            var savedRefreshedToken = await _tokenRepository.FindRefreshToken(memberGuid);
            if (savedRefreshedToken == null || savedRefreshedToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var newAccessToken = GenerateAccessToken(
                Guid.Parse(userId!),
                username!,
                Enum.Parse<MemberRole>(role!)
            );
            var newRefreshToken = new RefreshToken
            {
                TokenString = GenerateRefreshToken(),
                MemberId = Guid.Parse(userId!),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _tokenRepository.SaveRefreshToken(newRefreshToken);

            await _tokenRepository.RevokeRefreshToken(savedRefreshedToken);
            newRefreshToken.MemberId = Guid.Parse(userId!);
            await _tokenRepository.SaveRefreshToken(newRefreshToken);

            return newAccessToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}