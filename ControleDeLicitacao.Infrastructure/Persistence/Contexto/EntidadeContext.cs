using ControleDeLicitacao.Domain.Entities.Cadastros;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto;

public class EntidadeContext : DbContext
{
    public EntidadeContext(DbContextOptions<EntidadeContext> opts)
        : base(opts)
    {

    }

    public DbSet<Entidade> Entidades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração do Value Object Endereco
        modelBuilder.Entity<Entidade>(entity =>
        {
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
        });
    }
}
