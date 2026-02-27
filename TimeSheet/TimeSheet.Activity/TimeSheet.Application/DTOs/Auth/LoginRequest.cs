using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Auth
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
