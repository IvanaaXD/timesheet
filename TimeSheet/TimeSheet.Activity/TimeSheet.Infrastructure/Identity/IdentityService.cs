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

namespace TimeSheet.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IConfiguration _configuration;

        public IdentityService(IMemberRepository memberRepository, IConfiguration configuration)
        {
            _memberRepository = memberRepository;
            _configuration = configuration;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var member = await _memberRepository.FindMemberByUsernameAsync(request.Username);

            if (member == null)
            {
                return new AuthResult
                {
                    StatusCode = 404,
                };
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, member.Password);

            if (!isPasswordValid)
            {
                return new AuthResult
                {
                    StatusCode = 401,
                };
            }

            var accessToken = GenerateAccessToken(member);
            var refreshToken = GenerateRefreshToken(member);

            return new AuthResult
            {
                StatusCode = 200,
                Data = new AuthResponse
                {
                    Id = member.Id,
                    Username = member.Username,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        private string GenerateAccessToken(Member member)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                new Claim(ClaimTypes.Name, member.Username),
                new Claim(ClaimTypes.Role, member.Role.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(3),
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
    }
}