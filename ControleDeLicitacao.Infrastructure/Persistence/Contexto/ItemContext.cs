using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto;

public class ItemContext : DbContext
{
    public ItemContext(DbContextOptions<ItemContext> opts) : base(opts) { }

    public DbSet<Item> Itens { get; set; }
    public DbSet<ItemAssociativo> ItensAssociativos { get; set; }
    public DbSet<ItemNomeAssociativo> NomesAssociativos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
             .HasMany(i => i.ListaItens)
             .WithOne(ia => ia.ItemPai)
             .HasForeignKey(ia => ia.ItemPaiID)
             .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ItemAssociativo>()
            .HasOne(ia => ia.ItemFilho)
            .WithMany()
            .HasForeignKey(ia => ia.ItemFilhoID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Item>()
            .HasMany(i => i.ListaNomes)
            .WithOne(na => na.Item)
            .HasForeignKey(na => na.ItemID);
    }
}
