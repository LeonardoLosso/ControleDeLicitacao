using System.ComponentModel.DataAnnotations;

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

    [MaxLength(10)]
    public string NumEmpenho { get; set; }

    [Required]
    public int Unidade { get; set; }

    [Required]
    public int OrgaoID { get; set; }
    public int Status { get; set; }
    public DateTime? DataEmpenho { get; set; }

    [Required]
    public double Saldo { get; set; }

    [Required]
    public double Valor { get; set; }

    public ICollection<ItemDeEmpenho> Itens { get; set; }
}
