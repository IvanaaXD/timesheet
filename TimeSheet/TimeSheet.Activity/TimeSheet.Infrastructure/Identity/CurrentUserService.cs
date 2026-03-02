using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Application.DTOs.Auth;
using TimeSheet.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace TimeSheet.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Username => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public Guid UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var claim = user?.FindFirst(ClaimTypes.NameIdentifier) 
                            ?? user?.FindFirst("nameid")               
                            ?? user?.FindFirst("sub");                 

                if (claim == null || string.IsNullOrEmpty(claim.Value))
                {
                    return Guid.Empty;
                }

                return Guid.Parse(claim.Value);
            }
        }
    }
}