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
                endereco.Property(e => e.Logradouro).HasColumnName("Logradouro");
                endereco.Property(e => e.Bairro).HasColumnName("Bairro");
                endereco.Property(e => e.Cidade).HasColumnName("Cidade");
                endereco.Property(e => e.UF).HasColumnName("UF");
                endereco.Property(e => e.CEP).HasColumnName("CEP");
                endereco.Property(e => e.Numero).HasColumnName("Numero");
                endereco.Property(e => e.Complemento).HasColumnName("Complemento");
            });

            entity.HasMany(e => e.Permissoes)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey(p => p.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Permissao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tela).IsRequired();
            entity.Property(e => e.NomeRecurso).IsRequired();
            entity.Property(e => e.PermissaoRecurso).IsRequired();
        });
    }

}
