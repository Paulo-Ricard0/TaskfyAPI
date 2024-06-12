using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs.Usuario
{
	public class ResponseLoginTokenDTO : ResponseDTO
	{
		public string Token { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime Expiration { get; set; }

		[JsonIgnore]
		public override string Status { get; set; } = string.Empty;

		[JsonIgnore]
		public override string Message { get; set; } = string.Empty;
	}
}
