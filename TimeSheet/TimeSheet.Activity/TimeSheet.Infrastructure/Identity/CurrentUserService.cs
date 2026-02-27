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
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }
    }
}