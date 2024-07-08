using ControleDeLicitacao.Domain.Entities.Documentos.Ata;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto;

public class AtaContext : DbContext
{
    public AtaContext(DbContextOptions<AtaContext> opts): base(opts) { }

    public DbSet<AtaLicitacao> AtaLicitacao { get; set; }

    public DbSet<ItemDeAta> ItemDeAta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AtaLicitacao>()
            .HasMany(i => i.Itens)
            .WithOne(p => p.Ata)
            .HasForeignKey(i => i.AtaID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemDeAta>()
            .HasKey(i => new { i.AtaID, i.ID});
    }
}
