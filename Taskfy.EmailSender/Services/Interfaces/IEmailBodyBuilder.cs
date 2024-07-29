using MimeKit;
using Taskfy.EmailSender.Models;

namespace Taskfy.EmailSender.Services.Interfaces;
public interface IEmailBodyBuilder
{
	BodyBuilder Build(EmailDetails emailDetails);
}
