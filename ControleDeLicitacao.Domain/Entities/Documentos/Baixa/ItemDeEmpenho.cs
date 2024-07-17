using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa;

public class ItemDeEmpenho
{

    [Required]
    public int ID { get; set; }

    [Required]
    public int EmpenhoID { get; set; }

    [Required]
    public int BaixaID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [MaxLength(10)]
    public string Unidade { get; set; }

    [Required]
    public double QtdeEmpenhada { get; set; } // Total do empenho 

    [Required]
    public double QtdeEntregue { get; set; } // Somatoria das notas 

    [Required]
    public double QtdeAEntregar { get; set; } // QtdeEmpenhada - QtdeEntregue

    [Required]
    public double ValorEntregue { get; set; } // ValorUnitario * QtdeEntregue

    [Required]
    public double ValorUnitario { get; set; }

    [Required]
    public double Total { get; set; } // ValorUnitario * QtdeEmpenhada

    [ForeignKey(nameof(EmpenhoID))]
    public Empenho Empenho { get; set; }
}
