using MimeKit;
using Taskfy.EmailSender.Models;
using Taskfy.EmailSender.Services.Interfaces;

namespace Taskfy.EmailSender.Services;
public class EmailBodyBuilder : IEmailBodyBuilder
{
	public BodyBuilder Build(EmailDetails emailDetails)
	{
		var bodyBuilder = new BodyBuilder
		{
			HtmlBody = GetEmailBody(emailDetails)
		};
		return bodyBuilder;
	}

	private string GetEmailBody(EmailDetails emailDetails)
	{
		return emailDetails.Type switch
		{
			EmailType.UserCreated => $@"
			<h1>Olá {emailDetails.UserName}!</h1>
			<p>Estamos	muito felizes em tê-lo(a) conosco no Taskfy!</p>
			<p>Aqui estão algumas dicas para começar:</p>
			<p>Crie suas primeiras tarefas e organize seu dia. Explore nossas funcionalidades para gerenciar suas tarefas de forma eficiente. Se precisar de qualquer ajuda, não hesite em entrar em contato conosco.</p>
			<p>Bem-vindo(a) a bordo!</p>
			<p>Atenciosamente, Equipe Taskfy</p>",
			_ => "<h1>Desconsidere esse e-mail.</h1>"
		};
	}
}