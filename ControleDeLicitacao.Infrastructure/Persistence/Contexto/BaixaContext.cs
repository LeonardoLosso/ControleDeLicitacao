using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto
{
    public class BaixaContext : DbContext
    {
        public BaixaContext(DbContextOptions<BaixaContext> opts): base(opts) { }

        public DbSet<BaixaLicitacao> BaixaLicitacao { get; set; }
        public DbSet<ItemDeBaixa> ItemDeBaixa { get; set; }
        //----------------------------------------------------------------
        public DbSet<Empenho> Empenho { get; set; }
        public DbSet<ItemDeEmpenho> ItemDeEmpenho { get; set; }
        //----------------------------------------------------------------
        public DbSet<Nota> Nota { get; set; }
        public DbSet<ItemDeNota> ItemDeNota { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaixaLicitacao>()
                .HasMany(i => i.Itens)
                .WithOne(p => p.Baixa)
                .HasForeignKey(p => p.BaixaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemDeBaixa>()
                .HasKey(p => new { p.BaixaID, p.ID, p.ValorUnitario});


            modelBuilder.Entity<Empenho>()
                .HasMany(i => i.Itens)
                .WithOne(p => p.Empenho)
                .HasForeignKey(p => p.EmpenhoID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemDeEmpenho>()
                .HasKey(p => new { p.EmpenhoID, p.ID});

            modelBuilder.Entity<Nota>()
                .HasMany(i => i.Itens)
                .WithOne(p => p.Nota)
                .HasForeignKey(p => p.NotaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemDeNota>()
                .HasKey(p => new { p.NotaID, p.ID });
        }
    }
}
