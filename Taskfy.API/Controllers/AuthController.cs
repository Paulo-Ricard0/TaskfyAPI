using Microsoft.AspNetCore.Mvc;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Services.Auth;

namespace Taskfy.API.Controllers;

[Route("api/users/")]
[ApiController]
[Produces("application/json")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost]
	[Route("register")]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> Register([FromBody] RegistroModelDTO usuarioModel)
	{
		var response = await _authService.RegisterAsync(usuarioModel);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	[Route("login")]
	[ProducesResponseType(typeof(ResponseLoginTokenDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesDefaultResponseType]
	public async Task<IActionResult> Login([FromBody] LoginModelDTO usuarioModel)
	{
		var responseToken = await _authService.LoginAsync(usuarioModel);
		return StatusCode(responseToken.StatusCode, responseToken);
	}
}
