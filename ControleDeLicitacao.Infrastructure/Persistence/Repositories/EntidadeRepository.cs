using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.Infrastructure.Persistence.Repositories;

public class EntidadeRepository : Repository<Entidade>
{
    private readonly EntidadeContext _context;

    public EntidadeRepository(EntidadeContext context, LogContext logContext, UserKeeper userKeeper) : base(context, logContext, userKeeper)
    {
        _context = context;
    }

    public override async Task Editar(Entidade entity)
    {

        var oldVal = await Buscar().FirstOrDefaultAsync(e => e.ID == entity.ID);

        if (oldVal is null) throw new Exception("Não encontrado");

        _context.Entry(oldVal).CurrentValues.SetValues(entity);
        _context.Entry(oldVal.Endereco).CurrentValues.SetValues(entity.Endereco);


        await base.SalvaContexto();
    }
}
