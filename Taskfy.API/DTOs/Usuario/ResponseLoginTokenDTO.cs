using System.Text.Json.Serialization;

namespace Taskfy.API.DTOs.Usuario
{
	public class ResponseLoginTokenDTO : ResponseDTO
	{
		public string? Token { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? Expiration { get; set; }

		[JsonIgnore]
		public override string? Status { get; set; }

		[JsonIgnore]
		public override string? Message { get; set; }
	}
}
