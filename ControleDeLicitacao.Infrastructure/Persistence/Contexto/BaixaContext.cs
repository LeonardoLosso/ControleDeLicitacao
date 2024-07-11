using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto
{
    public class BaixaContext : DbContext
    {
        public BaixaContext(DbContextOptions<BaixaContext> opts): base(opts) { }

        public DbSet<BaixaLicitacao> BaixaLicitacao { get; set; }
        public DbSet<ItemDeBaixa> ItemDeBaixa { get; set; }
        public DbSet<Empenho> Empenho { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaixaLicitacao>()
                .HasMany(i => i.Itens)
                .WithOne(p => p.Baixa)
                .HasForeignKey(p => p.BaixaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaixaLicitacao>()
                .HasMany(i => i.Empenhos)
                .WithOne(p => p.Baixa)
                .HasForeignKey(p => p.BaixaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemDeBaixa>()
                .HasKey(p => new { p.BaixaID, p.ID});

        }
    }
}
