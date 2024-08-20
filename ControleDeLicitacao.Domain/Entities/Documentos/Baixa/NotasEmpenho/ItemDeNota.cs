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
    public double QtdeCaixa { get; set; } = 0;

    [Required]
    public double ValorUnitario { get; set; }

    [Required]
    public double ValorTotal { get; set; }
    public double ValorCaixa { get; set; } = 0;


    [ForeignKey(nameof(NotaID))]
    public Nota Nota { get; set; }
}
