using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Taskfy.API.Logs;
using Taskfy.API.Models;
using Taskfy.API.Services.Auth;
using Taskfy.API.Services.MessagesQueue;

namespace Taskfy.Tests.Unit.Auth.Services.Mocks;
public abstract class BaseUserServiceSetup
{
	protected readonly UserManager<Usuario> UserManagerMock;
	protected readonly IConfiguration ConfigurationMock;
	protected readonly ITokenService TokenServiceMock;
	protected readonly ILog LoggerMock;
	protected readonly IMessageQueueService MessageQueueServiceMock;
	protected readonly IAuthService AuthServiceMock;

	protected BaseUserServiceSetup()
	{
		UserManagerMock = Substitute.For<UserManager<Usuario>>(
				Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

		ConfigurationMock = Substitute.For<IConfiguration>();
		TokenServiceMock = Substitute.For<ITokenService>();
		LoggerMock = Substitute.For<ILog>();
		MessageQueueServiceMock = Substitute.For<IMessageQueueService>();

		AuthServiceMock = new AuthService(UserManagerMock, ConfigurationMock, TokenServiceMock, LoggerMock, MessageQueueServiceMock);
	}
}
