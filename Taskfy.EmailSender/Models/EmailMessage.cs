namespace Taskfy.EmailSender.Models;
public class EmailMessage
{
	public required string To { get; set; }
	public required string Subject { get; set; }
	public required EmailDetails Body { get; set; }
}
