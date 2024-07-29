using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.DTOs.Usuario;
using Taskfy.API.Models;

namespace Taskfy.Tests.Unit.ServicesMocks;

public static class MocksData
{
	public static UserMockData User { get; } = new UserMockData();
	public static TarefaMockData Tarefa { get; } = new TarefaMockData();
	public static AuthMockData Auth { get; } = new AuthMockData();

	public class UserMockData()
	{
		public Usuario CreateUsuario(RegistroModelDTO registroRequest) => new Usuario
		{
			Name = registroRequest.Name,
			Email = registroRequest.Email,
			UserName = registroRequest.Email,
			SecurityStamp = Guid.NewGuid().ToString()
		};

		public string GetUserId() => Guid.NewGuid().ToString();

		public ClaimsPrincipal GetClaimsPrincipal(string userId)
		{
			var claims = new[] { new Claim("UserId", userId) };
			return new ClaimsPrincipal(new ClaimsIdentity(claims));
		}

		public UserProjection GetUserProjection() => new UserProjection
		{
			Name = "testuser test",
			Email = "test@gmail.com"
		};

		// Login
		public LoginModelDTO GetLoginRequestDTO() => new LoginModelDTO
		{
			Email = "test@gmail.com",
			Password = "Test123@"
		};

		public Usuario GetUsuario() => new Usuario
		{
			Id = GetUserId(),
			Name = "testuser test",
			Email = "test@gmail.com",
		};

		// Registro
		public RegistroModelDTO GetRegistroRequestDTO() => new RegistroModelDTO
		{
			Name = "testuser test",
			Email = "test@gmail.com",
			Password = "Test123@"
		};
	}

	public class TarefaMockData()
	{
		public Tarefa CreateTarefa(string userId, TarefaRequestDTO tarefaRequest) => new Tarefa
		{
			Id = GetTarefaId(),
			Titulo = tarefaRequest.Titulo,
			Descricao = tarefaRequest.Descricao,
			Data_vencimento = tarefaRequest.Data_vencimento,
			Status = false,
			Usuario = null,
			Usuario_id = userId
		};

		public Guid GetTarefaId() => Guid.NewGuid();

		public Tarefa GetTarefa(Guid tarefaId, string userId) => new Tarefa
		{
			Id = tarefaId,
			Titulo = "Tarefa Existente",
			Descricao = "Descrição da tarefa existente",
			Data_vencimento = DateTime.Now.AddDays(2),
			Status = false,
			Usuario = null,
			Usuario_id = userId
		};

		public TarefaDTO GetTarefaDTO(Tarefa tarefa) => new TarefaDTO
		{
			Id = tarefa.Id,
			Titulo = tarefa.Titulo,
			Descricao = tarefa.Descricao,
			Data_vencimento = tarefa.Data_vencimento,
			Status = tarefa.Status,
			Usuario_id = tarefa.Usuario_id
		};

		public List<Tarefa> ListaTarefas(string userId) => new List<Tarefa>
		{
			new Tarefa
			{
				Id = Guid.NewGuid(),
				Titulo = "Tarefa 1",
				Descricao = "Descrição da tarefa 1",
				Data_vencimento = DateTime.Now.AddDays(1),
				Usuario = null,
				Usuario_id = userId,
				Status = false
			},
			new Tarefa
			{
				Id = Guid.NewGuid(),
				Titulo = "Tarefa 2",
				Descricao = "Descrição da tarefa 2",
				Data_vencimento = DateTime.Now.AddDays(2),
				Usuario = null,
				Usuario_id = userId,
				Status = true
			}
		};

		public TarefaRequestDTO GetTarefaRequestDTO() => new TarefaRequestDTO
		{
			Titulo = "Nova Tarefa",
			Descricao = "Descrição da nova tarefa",
			Data_vencimento = DateTime.Now.AddDays(1)
		};

		public Tarefa GetTarefaAtualizada(Guid tarefaId, TarefaRequestUpdateDTO tarefaRequestUpdate, string userId) => new Tarefa
		{
			Id = tarefaId,
			Titulo = tarefaRequestUpdate.Titulo,
			Descricao = tarefaRequestUpdate.Descricao,
			Data_vencimento = tarefaRequestUpdate.Data_vencimento,
			Status = tarefaRequestUpdate.Status,
			Usuario = null,
			Usuario_id = userId
		};

		public TarefaRequestUpdateDTO GetTarefaRequestUpdateDTO() => new TarefaRequestUpdateDTO
		{
			Titulo = "Tarefa Atualizada",
			Descricao = "Descrição atualizada",
			Data_vencimento = DateTime.Now.AddDays(3),
			Status = true
		};
	}

	public class AuthMockData()
	{
		public JwtSecurityToken GetJwtToken(string userId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("UserId", userId),
				}),
				Expires = DateTime.UtcNow.AddMinutes(30),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7b4093b95b3c848a3c3b961da2203c848a09093c")), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
			return token;
		}
	}
}
