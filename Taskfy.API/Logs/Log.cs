using System.Text.Json;
using Taskfy.API.DTOs.Log;

namespace Taskfy.API.Logs;

public class Log : ILog
{
	public void LogToFile(string title, string logMessage)
	{
		string baseDirectory = GetProjectBaseDirectory();

		string logDirectory = Path.Combine(baseDirectory, "Logs", "LogFiles");

		if (!Directory.Exists(logDirectory))
		{
			Directory.CreateDirectory(logDirectory);
		}

		string nomeDoArquivo = Path.Combine(logDirectory, DateTime.Now.ToString("ddMMyyyy") + ".txt");

		using (StreamWriter swLog = new(nomeDoArquivo, append: true))
		{
			swLog.WriteLine("Log:");
			swLog.WriteLine($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
			swLog.WriteLine("Título do log: {0}", title);
			swLog.WriteLine("Mensagem: {0}", logMessage);
			swLog.WriteLine("-----------------------------------");
			swLog.WriteLine("");
		}
	}

	public void LogErrorToFile(string title, string logContent)
	{
		string baseDirectory = GetProjectBaseDirectory();

		string logDirectory = Path.Combine(baseDirectory, "Logs", "LogFiles");

		if (!Directory.Exists(logDirectory))
		{
			Directory.CreateDirectory(logDirectory);
		}

		string nomeDoArquivo = Path.Combine(logDirectory, DateTime.Now.ToString("ddMMyyyy") + ".txt");

		LogErrorDTO logContentDeserialized = DeserializeLogContent(logContent);

		using (StreamWriter swLog = new(nomeDoArquivo, append: true))
		{
			swLog.WriteLine("Log:");
			swLog.WriteLine($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
			swLog.WriteLine("Título do log: {0}", title);
			swLog.WriteLine("Mensagem: {0}", logContentDeserialized.Message);
			swLog.WriteLine("");
			swLog.WriteLine("Erro: {0}", logContentDeserialized.Error);
			swLog.WriteLine("");
			swLog.WriteLine("stackTrace: {0}", logContentDeserialized.StackTrace);
			swLog.WriteLine("");
			swLog.WriteLine("source: {0}", logContentDeserialized.Source);
			swLog.WriteLine("");
			swLog.WriteLine("exception: {0}", logContentDeserialized.Exception);
			swLog.WriteLine("");
			swLog.WriteLine("-----------------------------------");
			swLog.WriteLine("");
		}
	}

	private static string GetProjectBaseDirectory()
	{
		string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

		DirectoryInfo? directoryInfo = new DirectoryInfo(currentDirectory);
		while (directoryInfo != null && directoryInfo.Name != "Taskfy.API")
		{
			directoryInfo = directoryInfo.Parent;
		}

		if (directoryInfo == null)
		{
			throw new DirectoryNotFoundException("Não foi possível encontrar o diretório base do projeto.");
		}

		return directoryInfo.FullName;
	}

	private static LogErrorDTO DeserializeLogContent(string logContent)
	{
		var log = JsonSerializer.Deserialize<JsonElement>(logContent);

		string message = log.GetProperty("message").GetString() ?? "";
		string error = log.GetProperty("error").GetString() ?? "";
		string stackTrace = log.GetProperty("stackTrace").GetString() ?? "";
		string exception = log.GetProperty("exception").GetString() ?? "";
		string source = log.GetProperty("source").GetString() ?? "";

		return new LogErrorDTO
		{
			Message = message,
			Error = error,
			StackTrace = stackTrace,
			Exception = exception,
			Source = source
		};
	}
}
