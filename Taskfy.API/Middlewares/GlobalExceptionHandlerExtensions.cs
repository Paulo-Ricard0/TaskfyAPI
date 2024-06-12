namespace Taskfy.API.Middlewares;

public static class GlobalExceptionHandlerExtensions
{
	public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
	}
}
