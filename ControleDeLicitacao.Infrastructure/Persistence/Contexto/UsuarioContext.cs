using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto;

public class UsuarioContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Users;
    public DbSet<Permissao> Permissoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.UserName).IsUnique();

            entity.OwnsOne(e => e.Endereco, endereco =>
            {
                endereco.Property(e => e.Logradouro).HasColumnName("Logradouro").IsRequired(false);
                endereco.Property(e => e.Bairro).HasColumnName("Bairro").IsRequired(false);
                endereco.Property(e => e.Cidade).HasColumnName("Cidade").IsRequired(false);
                endereco.Property(e => e.UF).HasColumnName("UF").IsRequired(false);
                endereco.Property(e => e.CEP).HasColumnName("CEP").IsRequired(false);
                endereco.Property(e => e.Numero).HasColumnName("Numero").IsRequired(false);
                endereco.Property(e => e.Complemento).HasColumnName("Complemento").IsRequired(false);
            });

            entity.HasMany(e => e.Permissoes)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey(p => p.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

            entity.Property(p => p.Nome)
            .IsRequired(false)
            .HasMaxLength(50);

            entity.Property(p => p.CPF)
            .IsRequired(false)
            .HasMaxLength(11);

            entity.Property(p => p.Telefone)
            .IsRequired(false)
            .HasMaxLength(11);
        });

        modelBuilder.Entity<Permissao>(entity =>
        {
            entity.HasKey(p => new { p.RecursoId, p.UsuarioId });
            entity.Property(e => e.RecursoId).IsRequired();
            entity.Property(e => e.PermissaoRecurso).IsRequired();
        });
    }

}
