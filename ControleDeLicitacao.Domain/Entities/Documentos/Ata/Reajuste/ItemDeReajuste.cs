using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;

public class ItemDeReajuste
{
    [Required]
    public int ID { get; set; }

    [Required]
    public int AtaID { get; set; }

    [Required]
    public int ReajusteID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [MaxLength(10)]
    public string Unidade { get; set; }

    [Required]
    public double Quantidade { get; set; }

    [Required]
    public double ValorUnitario { get; set; }

    [Required]
    public double ValorTotal { get; set; }
    public double Desconto { get; set; }


    [JsonIgnore]
    [ForeignKey(nameof(ReajusteID))]
    public Reajuste Reajuste { get; set; }
}
