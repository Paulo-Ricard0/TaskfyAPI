using RabbitMQ.Client;
using Taskfy.EmailSender.Services;
using Taskfy.EmailSender.Services.Interfaces;
using Taskfy.EmailSender.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<EmailWorker>();

builder.Services.AddSingleton<IConnection>(_ =>
{
	var factory = new ConnectionFactory() { HostName = "localhost" };
	return factory.CreateConnection();
});

builder.Services.AddSingleton<IModel>(sp =>
{
	var connection = sp.GetRequiredService<IConnection>();
	return connection.CreateModel();
});

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IEmailBodyBuilder, EmailBodyBuilder>();

var host = builder.Build();
await host.RunAsync();
