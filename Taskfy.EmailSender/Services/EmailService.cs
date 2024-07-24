using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Taskfy.EmailSender.Models;
using Taskfy.EmailSender.Services.Interfaces;

namespace Taskfy.EmailSender.Services;
public class EmailService : IEmailService
{
	private readonly ILogger<EmailService> _logger;
	private readonly IConfiguration _configuration;
	private readonly IEmailBodyBuilder _bodyBuilder;

	public EmailService(ILogger<EmailService> logger, IConfiguration configuration, IEmailBodyBuilder bodyBuilder)
	{
		_logger = logger;
		_configuration = configuration;
		_bodyBuilder = bodyBuilder;
	}

	public async Task SendEmailAsync(EmailMessage emailMessage)
	{
		var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

		if (smtpSettings == null)
		{
			_logger.LogError("SMTP settings are not configured properly.");
			throw new NullReferenceException("SMTP settings are not configured properly.");
		}

		var email = new MimeMessage();
		email.From.Add(new MailboxAddress(smtpSettings.Name, smtpSettings.FromEmail));
		email.To.Add(new MailboxAddress(emailMessage.Body.UserName, emailMessage.To));
		email.Subject = emailMessage.Subject;

		var bodyBuilder = _bodyBuilder.Build(emailMessage.Body);

		email.Body = bodyBuilder.ToMessageBody();

		using (var client = new SmtpClient())
		{
			try
			{
				SecureSocketOptions secureSocketOptions = smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

				await client.ConnectAsync(smtpSettings.Host, smtpSettings.Port, secureSocketOptions);
				await client.AuthenticateAsync(smtpSettings.FromEmail, smtpSettings.Password);
				await client.SendAsync(email);
				await client.DisconnectAsync(true);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error sending email to {To}", emailMessage.To);
				throw new NullReferenceException($"Error sending email to {emailMessage.To}");
			}
		}
	}
}
