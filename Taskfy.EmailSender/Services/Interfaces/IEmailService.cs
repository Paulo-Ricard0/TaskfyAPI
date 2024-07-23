using Taskfy.EmailSender.Models;

namespace Taskfy.EmailSender.Services.Interfaces;
public interface IEmailService
{
	Task SendEmailAsync(EmailMessage emailMessage);
}
