using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Taskfy.API.Services.Auth;

public interface ITokenService
{
	JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config);
};
