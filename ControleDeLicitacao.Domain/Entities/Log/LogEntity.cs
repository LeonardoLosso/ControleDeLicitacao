using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Log;

public class LogEntity
{
    [Key] 
    public int Id { get; set; }

    [MaxLength(10)]
    public string? TipoRequest { get; set; }

    [MaxLength(30)]
    public string? Path { get; set;}

    [MaxLength(50)]
    public string? UserName { get; set; }

    public DateTime? Horario { get; set; }

    [Column(TypeName = "text")]
    public string? Operacao { get; set; }

    public LogEntity(int id, string? tipoRequest, string? path, string? userName, DateTime? horario, string? operacao)
    {
        Id = id;
        TipoRequest = tipoRequest;
        Path = path;
        UserName = userName;
        Horario = horario;
        Operacao = operacao;
    }
}
