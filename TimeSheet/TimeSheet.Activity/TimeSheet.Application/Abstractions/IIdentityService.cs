using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Application.DTOs.Auth;

namespace TimeSheet.Application.Abstractions
{
    public interface IIdentityService
    {
        Task<AuthResult> LoginAsync(LoginRequest request);
    }
}