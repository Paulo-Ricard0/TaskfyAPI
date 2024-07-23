namespace Taskfy.EmailSender.Models;
public class SmtpSettings
{
	public required string Host { get; set; }
	public required int Port { get; set; }
	public required string Name { get; set; }
	public required string Password { get; set; }
	public required string FromEmail { get; set; }
	public required bool EnableSsl { get; set; }
}
