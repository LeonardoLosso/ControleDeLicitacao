using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa;

public class Empenho
{
    [Key]
    public int ID { get; set; }

    [Required]
    public int BaixaID { get; set; }

    [Required]
    [MaxLength(10)]
    public string Edital { get; set; }
    public int Status { get; set; }
    public DateTime? DataEmpenho { get; set; }

    [Required]
    public double Saldo { get; set; }

    [Required]
    public double Valor { get; set; }

    //public ICollection<ItemDeBaixa> Itens { get; set; }
    //public ICollection<Notas> Notas { get; set; }

    [ForeignKey(nameof(BaixaID))]
    public BaixaLicitacao Baixa { get; set; }
}
