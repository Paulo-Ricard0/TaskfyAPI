using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;
using Taskfy.API.Services.Auth;

namespace Taskfy.Tests.Unit.Auth.Services
{
	public class RegisterServiceTests
	{
		[Fact]
		public async Task Registra_QuandoUsuarioNaoExiste_CriaUsuarioComSucesso()
		{
			// Arrange
			var usuarioModel = new RegistroModelDTO
			{
				UserName = "testuser",
				Email = "test@gmail.com",
				Password = "Test123@"
			};

			var userManager = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

			userManager.FindByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<Usuario?>(null));
			userManager.CreateAsync(Arg.Any<Usuario>(), Arg.Any<string>()).Returns(Task.FromResult(IdentityResult.Success));

			var configuration = Substitute.For<IConfiguration>();
			var authService = new AuthService(userManager, configuration);

			// Act
			var result = await authService.RegisterAsync(usuarioModel);

			// Assert
			result.StatusCode.Should().Be(StatusCodes.Status201Created);
			result.Status.Should().Be("Sucesso");
			result.Message.Should().Be("Usuário criado com sucesso!");
		}
	}
}
