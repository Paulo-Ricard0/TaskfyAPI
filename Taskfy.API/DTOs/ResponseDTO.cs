using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs;

public class ResponseDTO
{
	public string? Status { get; set; }
	public string? Message { get; set; }

	[JsonIgnore]
	public int StatusCode { get; set; }
}
