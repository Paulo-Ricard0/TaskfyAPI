using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System.Text;
using Taskfy.API.Data;
using Taskfy.API.DTOs.Mappings;
using Taskfy.API.Logs;
using Taskfy.API.Middlewares;
using Taskfy.API.Models;
using Taskfy.API.Repositories;
using Taskfy.API.Repositories.Tarefas;
using Taskfy.API.Repositories.Usuarios;
using Taskfy.API.Services.Auth;
using Taskfy.API.Services.MessagesQueue;
using Taskfy.API.Services.Tarefas;
using Taskfy.API.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Configura os serviços
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configura o pipeline HTTP
Configure(app);

app.Run();

void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
	// Adiciona Serviços ao container
	services.AddControllers();
	services.AddEndpointsApiExplorer();
	services.AddSwaggerGen(options =>
	{
		options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Type = SecuritySchemeType.Http,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Name = "Authorization",
			Description = "Por favor, insira o token JWT retornado pelo login aqui para autenticar-se",
		});
		options.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					}
				},
				Array.Empty<string>()
			}
		});
	});

	// Configura Identity
	services.AddIdentity<Usuario, IdentityRole>()
		.AddEntityFrameworkStores<AppDbContext>()
		.AddDefaultTokenProviders();

	// Configura a conexão do banco de dados
	services.AddDbContext<AppDbContext>(options =>
		options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

	// Adiciona Automapper
	services.AddAutoMapper(typeof(TarefaDTOMappingProfile));

	// Adiciona serviços de aplicação
	services.AddScoped<ILog, Log>();
	services.AddScoped<IAuthService, AuthService>();
	services.AddScoped<ITokenService, TokenService>();
	services.AddScoped<IUsuarioRepository, UsuarioRepository>();

	services.AddScoped<ITarefaService, TarefaService>();
	services.AddScoped<ITarefaRepository, TarefaRepository>();
	services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
	services.AddScoped<IUnitOfWork, UnitOfWork>();

	services.AddSingleton<IConnection>(_ =>
	{
		var factory = new ConnectionFactory() { HostName = "localhost" };
		return factory.CreateConnection();
	});

	services.AddSingleton<IModel>(sp =>
	{
		var connection = sp.GetRequiredService<IConnection>();
		return connection.CreateModel();
	});

	services.AddSingleton<IMessageQueueService, MessageQueueService>();

	// Configura JWT Authentication
	var jwtSettings = configuration.GetSection("JWT");
	services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.RequireHttpsMetadata = true;
		options.SaveToken = true;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings["ValidIssuer"],
			ValidAudience = jwtSettings["ValidAudience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
		};
	});
}

void Configure(WebApplication app)
{
	// Configura requisições HTTP
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();
	app.UseGlobalExceptionHandler();
	app.UseAuthentication();
	app.UseAuthorization();
	app.MapControllers();
}