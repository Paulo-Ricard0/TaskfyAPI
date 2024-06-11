using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taskfy.API.Models;

namespace Taskfy.API.Data;

public class AppDbContext : IdentityDbContext<Usuario>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<Tarefa> Tarefas { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Configura o relacionamento entre Usuario e Tarefa
		modelBuilder.Entity<Tarefa>()
			.HasOne(t => t.Usuario)
			.WithMany(u => u.Tarefas)
			.HasForeignKey(t => t.Usuario_id)
			.OnDelete(DeleteBehavior.Cascade);

		// Configura o valor padrão para o Status como false
		modelBuilder.Entity<Tarefa>()
			.Property(t => t.Status)
			.HasDefaultValue(false);

		// Configura a geração automática do Id como UUID
		modelBuilder.Entity<Tarefa>()
			.Property(t => t.Id)
			.HasDefaultValueSql("NEWID()");
	}
}
