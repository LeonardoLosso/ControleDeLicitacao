using ControleDeLicitacao.Domain.Entities.Documentos.Ata;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class AtaRepository : Repository<AtaLicitacao>
{
    private readonly AtaContext _context;

    public AtaRepository(AtaContext context) : base(context)
    {
        _context = context;
    }
}
