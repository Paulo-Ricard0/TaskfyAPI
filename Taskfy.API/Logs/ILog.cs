namespace Taskfy.API.Logs;

public interface ILog
{
	void LogToFile(string title, string logMessage);
	void LogErrorToFile(string title, string logContent);
}
