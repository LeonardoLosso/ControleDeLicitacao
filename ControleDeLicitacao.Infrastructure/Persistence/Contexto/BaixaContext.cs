using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto
{
    public class BaixaContext : DbContext
    {
        public BaixaContext(DbContextOptions opts): base(opts) { }

        public DbSet<Baixa> BaixaLicitacao { get; set; }
    }
}
