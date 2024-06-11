using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs;

public class ResponseDTO
{
	public virtual string? Status { get; set; }
	public virtual string? Message { get; set; }

	[JsonIgnore]
	public int StatusCode { get; set; }
}
