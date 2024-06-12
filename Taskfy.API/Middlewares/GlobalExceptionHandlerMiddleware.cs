using System.Net;
using System.Text.Json;
using Taskfy.API.Logs;

namespace Taskfy.API.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
	private readonly RequestDelegate _next;

	public GlobalExceptionHandlerMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			var logger = context.RequestServices.GetRequiredService<ILog>();
			await HandleExceptionAsync(context, ex, logger);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILog logger)
	{
		var statusCode = HttpStatusCode.InternalServerError;
		var response = new
		{
			status = "Erro",
			message = "Ocorreu um erro interno no servidor.",
			error = exception.Message,
		};

		var log = new
		{
			message = "Ocorreu um erro interno no servidor.",
			error = exception.Message,
			stackTrace = exception.StackTrace,
			exception = exception.InnerException,
			source = exception.Source,
		};

		var responseContent = JsonSerializer.Serialize(response);
		var logContent = JsonSerializer.Serialize(log);

		logger.LogErrorToFile("Erro interno do servidor.", logContent);

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)statusCode;

		return context.Response.WriteAsync(responseContent);
	}
}
