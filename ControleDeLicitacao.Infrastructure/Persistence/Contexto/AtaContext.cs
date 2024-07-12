using ControleDeLicitacao.Domain.Entities.Documentos.Ata;
using ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto;

public class AtaContext : DbContext
{
    public AtaContext(DbContextOptions<AtaContext> opts): base(opts) { }

    public DbSet<AtaLicitacao> AtaLicitacao { get; set; }
    public DbSet<ItemDeAta> ItemDeAta { get; set; }

    public DbSet<Reajuste> Reajuste { get; set; }
    public DbSet<ItemDeReajuste> ItemDeReajuste { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AtaLicitacao>()
            .HasMany(i => i.Itens)
            .WithOne(p => p.Ata)
            .HasForeignKey(i => i.AtaID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemDeAta>()
            .HasKey(i => new { i.AtaID, i.ID, i.ValorUnitario});

        modelBuilder.Entity<Reajuste>()
            .HasMany(r => r.Itens)
            .WithOne(i => i.Reajuste)
            .HasForeignKey(i => i.ReajusteID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemDeReajuste>()
            .HasKey(ir => new { ir.AtaID, ir.ReajusteID, ir.ID });

        modelBuilder.Entity<ItemDeReajuste>()
            .HasOne(ir => ir.Reajuste)
            .WithMany(r => r.Itens)
            .HasForeignKey(ir => ir.ReajusteID);
    }
}
