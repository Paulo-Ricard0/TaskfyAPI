using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs.Usuario
{
	public class ResponseLoginTokenDTO : ResponseDTO
	{
		public required string Token { get; set; }
		public required string RefreshToken { get; set; }
		public DateTime Expiration { get; set; }

		[JsonIgnore]
		public override string Status { get; set; } = string.Empty;
		[JsonIgnore]
		public override string Message { get; set; } = string.Empty;
	}
}
