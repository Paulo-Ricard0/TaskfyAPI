namespace Taskfy.API.DTOs;

public class GlobalErrorResponseDTO : ResponseDTO
{
	public required string Error { get; set; }
}
