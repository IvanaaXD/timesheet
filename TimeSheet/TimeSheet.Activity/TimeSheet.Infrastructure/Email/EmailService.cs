using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.Common.Models;

namespace TimeSheet.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body 
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error while sending e-mail: " + ex.Message);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendWelcomeEmailAsync(string receiver, string password)
        {
            string subject = "Welcome to TimeSheet - Your Access Data";
            string body = GetEmailTemplate("Welcome to TimeSheet!",
                "Your account has been successfully created. Use the password below to log in to your account.",
                password,
                "If you did not request this change, please contact support immediately.");

            await SendEmailAsync(receiver, subject, body);
        }

        public async Task SendPasswordUpdatedEmailAsync(string receiver, string password)
        {
            string subject = "TimeSheet - Password Updated";
            string body = GetEmailTemplate("Password Updated",
                "Your account password has been successfully updated by an administrator.",
                password,
                "If you did not request this change, please contact support immediately.");

            await SendEmailAsync(receiver, subject, body);
        }

        private string GetEmailTemplate(string title, string message, string password, string note)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; border-radius: 10px; overflow: hidden;'>
                <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-bottom: 2px solid #007bff;'>
                    <h1 style='color: #333; margin: 0;'>TimeSheet System</h1>
                </div>
                <div style='padding: 30px; line-height: 1.6; color: #444;'>
                    <h2 style='color: #007bff;'>{title}</h2>
                    <p>{message}</p>
                    <div style='background-color: #f4f4f4; padding: 15px; border-radius: 5px; text-align: center; margin: 20px 0;'>
                        <span style='font-size: 12px; color: #888; display: block; margin-bottom: 5px;'>TEMPORARY PASSWORD</span>
                        <strong style='font-size: 24px; color: #333; letter-spacing: 2px;'>{password}</strong>
                    </div>
                    <p style='font-size: 0.9em; color: #666; font-style: italic;'>{note}</p>
                </div>
                <div style='background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 0.8em; color: #aaa;'>
                    &copy; {DateTime.Now.Year} TimeSheet Application | Vega IT Internship Project
                </div>
            </div>";
        }
    }
}