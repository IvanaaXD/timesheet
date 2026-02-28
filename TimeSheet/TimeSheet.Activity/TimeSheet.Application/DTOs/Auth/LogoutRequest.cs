using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Auth
{
    public class LogoutRequest
    {
        public string TokenString { get; set; }
    }
}
