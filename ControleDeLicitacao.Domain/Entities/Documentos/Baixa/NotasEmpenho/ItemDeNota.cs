using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;

public class ItemDeNota
{
    [Required]
    public int ID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [Required]
    public int NotaID { get; set; }

    [Required]
    public int EmpenhoID { get; set; }

    [MaxLength(10)]
    public string Unidade { get; set; }

    [Required]
    public double Quantidade { get; set; }

    [Required]
    public double ValorUnitario { get; set; }

    [Required]
    public double ValorTotal { get; set; }

    [ForeignKey(nameof(NotaID))]
    public Nota Nota { get; set; }
}
