using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs;

public class ResponseDTO
{
	public virtual string Status { get; set; } = string.Empty;
	public virtual string Message { get; set; } = string.Empty;

	[JsonIgnore]
	public int StatusCode { get; set; }
}
