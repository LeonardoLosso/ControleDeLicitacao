using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Ata;

public class ItemDeAta
{
    [Required]
    public int ID { get; set; }

    [Required]
    public int AtaID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [MaxLength(10)]
    public string Unidade { get; set;}

    [Required]
    public double Quantidade { get; set;}

    [Required]
    public double ValorUnitario { get; set; }

    [Required]
    public double ValorTotal { get; set; }

    public double Desconto { get;set; }
}
