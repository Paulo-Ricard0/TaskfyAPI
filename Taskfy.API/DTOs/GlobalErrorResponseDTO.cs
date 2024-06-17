namespace Taskfy.API.DTOs;

public class GlobalErrorResponseDTO : ResponseDTO
{
	public string Error { get; set; } = string.Empty;
}
