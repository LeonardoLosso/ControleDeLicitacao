using ControleDeLicitacao.Domain.Entities.Log;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Contexto;

public class LogContext : DbContext
{
    public LogContext(DbContextOptions<LogContext> opts) : base(opts) { }

    public DbSet<LogEntity> SystemLog { get; set; }

}
