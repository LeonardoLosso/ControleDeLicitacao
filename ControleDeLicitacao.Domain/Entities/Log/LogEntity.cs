using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Log;

public class LogEntity
{
    [Key] 
    public int Id { get; set; }

    public int? RecordId { get; set; }

    [MaxLength(10)]
    public string? TipoRequest { get; set; }

    [MaxLength(30)]
    public string? Path { get; set;}

    [MaxLength(50)]
    public string? UserName { get; set; }

    public DateTime? Horario { get; set; }

    [MaxLength(20)]
    public string? Operacao { get; set; }

    [Column(TypeName = "text")]

    public string? NewValue { get; set; }

    [Column(TypeName = "text")]
    public string? OldValue { get; set; }

}
