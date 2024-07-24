using MimeKit;
using Taskfy.EmailSender.Models;
using Taskfy.EmailSender.Services.Interfaces;

namespace Taskfy.EmailSender.Services;
public class EmailBodyBuilder : IEmailBodyBuilder
{
	public BodyBuilder Build(EmailDetails emailDetails)
	{
		var dataFormatada = emailDetails.DataVencimentoTarefa.ToString("dd/MM/yyyy");

		emailDetails.DataVencimentoFormatada = dataFormatada;

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
			<h2>Olá {emailDetails.UserName}!</h2>
			</br>
			<h3>Estamos muito felizes em tê-lo(a) conosco no Taskfy!</h3>
			</br>
			<h4>Aqui estão algumas dicas para começar:</h4>
			<ul>
				<li>Crie suas primeiras tarefas e organize seu dia.</li>
				<li>Explore nossas funcionalidades para gerenciar suas tarefas de forma eficiente.</li>
				<li>Se precisar de qualquer ajuda, não hesite em entrar em contato conosco.</li>
			</ul>
			</br>
			<h5>Bem-vindo(a) a bordo!</h5>
			<p>Equipe Taskfy.</p>",
			EmailType.TaskCreated => $@"
			<h2>Olá {emailDetails.UserName}!</h2>
			</br>
			<h3>Uma nova tarefa foi criada em sua conta no Taskfy.</h3>
			</br>
			<p>Aqui estão os detalhes da tarefa:</p>
			<ul>
				<li>Título: {emailDetails.TituloTarefa}</li>
				<li>Descrição: {emailDetails.DescricaoTarefa}</li>
				<li>Data de vencimento: {emailDetails.DataVencimentoFormatada}</li>
			</ul>
			</br>
			<h5>Você pode acessar a tarefa e gerenciá-la através do nosso aplicativo.</h5>
			<p>Equipe Taskfy.</p>",
			EmailType.TaskUpdated => $@"
			<h2>Olá {emailDetails.UserName}!</h2>
			</br>
			<h3>A tarefa a seguir foi atualizada em sua conta no Taskfy.</h3>
			</br>
			<p>Aqui estão os detalhes atualizados da tarefa:</p>
			<ul>
				<li>Título: {emailDetails.TituloTarefa}</li>
				<li>Descrição: {emailDetails.DescricaoTarefa}</li>
				<li>Data de vencimento: {emailDetails.DataVencimentoFormatada}</li>
			</ul>
			</br>
			<h5>Você pode acessar a tarefa e verificar as atualizações através do nosso aplicativo.</h5>
			<p>Equipe Taskfy.</p>",
			_ => "<h1>Olá! por favor, desconsidere esse e-mail.</h1>"
		};
	}
}