using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa;

public class ItemDeBaixa
{
    [Required]
    public int ID { get; set; }

    [Required]
    public int BaixaID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [MaxLength(10)]
    public string Unidade { get; set; }

    [Required]
    public double QtdeEmpenhada { get; set; } = 0;

    [Required]
    public double QtdeLicitada { get; set; } = 0;

    [Required]
    public double QtdeAEmpenhar { get; set; } = 0;

    [Required]
    public double ValorEmpenhado { get; set; } = 0;

    [Required]
    public double ValorLicitado { get; set; } = 0;

    [Required]
    public double Saldo { get; set; } = 0;
    public double Desconto { get; set; }

    [Required]
    public double ValorUnitario { get; set; }

    [ForeignKey(nameof(BaixaID))]
    public BaixaLicitacao Baixa { get; set; }
}
