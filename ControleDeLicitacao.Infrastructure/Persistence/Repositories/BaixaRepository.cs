using ControleDeLicitacao.Domain.Entities.Documentos.Baixa;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class BaixaRepository : Repository<Baixa>
{
    private readonly BaixaContext _context;

    public BaixaRepository(BaixaContext context) : base(context)
    {
        _context = context;
    }
}
