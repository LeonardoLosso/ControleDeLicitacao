using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Ata.Reajuste;

public class Reajuste
{
    [Key]
    public int ID { get; set; }

    [Required]
    public int AtaID { get; set; }

    [Required]
    public DateTime Data { get; set; }
    public ICollection<ItemDeReajuste> Itens { get; set; }
}
