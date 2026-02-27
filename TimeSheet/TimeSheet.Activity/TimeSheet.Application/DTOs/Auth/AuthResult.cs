using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Auth
{
	public class AuthResult
	{
		public int StatusCode { get; set; }
		public AuthResponse Data { get; set; } 
	}
}
