using RabbitMQ.Client;
using Taskfy.NotificationService.Services;
using Taskfy.NotificationService.Services.Interfaces;
using Taskfy.NotificationService.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

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
builder.Services.AddSingleton<INotificationHandler, NotificationHandler>();

var host = builder.Build();
await host.RunAsync();
