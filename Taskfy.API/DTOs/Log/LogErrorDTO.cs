namespace Taskfy.API.DTOs.Log;

public class LogErrorDTO
{
	public string Message { get; set; } = string.Empty;
	public string Error { get; set; } = string.Empty;
	public string StackTrace { get; set; } = string.Empty;
	public string Exception { get; set; } = string.Empty;
	public string Source { get; set; } = string.Empty;
}
