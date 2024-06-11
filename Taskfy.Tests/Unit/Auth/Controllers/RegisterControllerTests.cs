using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Taskfy.API.Controllers;
using Taskfy.API.DTOs;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Services.Auth;

namespace Taskfy.Tests.Unit.Auth.Controllers
{
	public class RegisterControllerTests
	{
		[Fact]
		public async Task Verifica_UsuarioCriado_Retorna201CreatedStatusCode()
		{
			// Arrange
			var usuarioModel = new RegistroModelDTO
			{
				UserName = "testuser",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var responseDto = new ResponseDTO
			{
				Status = "Sucesso",
				Message = "Usuário criado com sucesso!",
				StatusCode = StatusCodes.Status201Created
			};

			var authService = Substitute.For<IAuthService>();
			authService.RegisterAsync(usuarioModel).Returns(Task.FromResult(responseDto));

			var controller = new AuthController(authService);

			// Act
			var result = await controller.Register(usuarioModel) as ObjectResult;

			// Assert
			result.Should().NotBeNull();
			result.StatusCode.Should().Be(StatusCodes.Status201Created);
			result.Value.Should().BeEquivalentTo(responseDto);
		}
	}
}
