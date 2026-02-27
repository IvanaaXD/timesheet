using System.Threading.Tasks;

namespace TimeSheet.Application.Abstractions
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiver, string subject, string body);
        Task SendWelcomeEmailAsync(string receiver, string password);
        Task SendPasswordUpdatedEmailAsync(string receiver, string password);
    }
}