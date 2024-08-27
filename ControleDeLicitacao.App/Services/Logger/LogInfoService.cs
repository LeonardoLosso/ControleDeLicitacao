
using ControleDeLicitacao.Domain.Entities.Log;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;

namespace ControleDeLicitacao.App.Services.Logger;

public class LogInfoService
{
    private readonly LogContext _context;

    private string _operacao;
    private string _userId;
    private string _httpMethod;
    private string _requestPath;

    private DateTime _requestTime;

    public LogInfoService(LogContext context)
    {
        _context = context;
    }

    public string UserId
    {
        get { return _userId; }
    }
    public string Operacao
    {
        get { return _operacao; }

    }
    public string HttpMethod
    {
        get { return _httpMethod; }
    }
    public string RequestPath
    {
        get { return _requestPath; }
    }
    public DateTime RequestTime
    {
        get { return _requestTime; }
    }

    public void SetRequestInfo(string userId, string method, string path, DateTime requestTime)
    {
        _userId = userId;
        _httpMethod = method;
        _requestPath = path;
        _requestTime = requestTime;
    }
    public void SetOperacao(string operacao)
    {
        _operacao = operacao;
    }
        
    public async Task SalvarLogAsync()
    {
        var log = new LogEntity(0, _httpMethod, _requestPath, _userId, _requestTime, _operacao);

        _context.Log.Add(log);
        await _context.SaveChangesAsync();
    }
}
