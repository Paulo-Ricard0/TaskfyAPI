using NSubstitute;
using Taskfy.API.Controllers;
using Taskfy.API.Services.Auth;

namespace Taskfy.Tests.Unit.Auth.Controllers.BaseMocks;
public abstract class BaseAuthControllerSetup
{
	protected readonly IAuthService AuthServiceMock;
	protected readonly AuthController AuthControllerMock;

	protected BaseAuthControllerSetup()
	{
		AuthServiceMock = Substitute.For<IAuthService>();
		AuthControllerMock = new AuthController(AuthServiceMock);
	}
}
