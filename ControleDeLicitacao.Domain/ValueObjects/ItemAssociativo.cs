using ControleDeLicitacao.Domain.Entities.Cadastros;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.ValueObjects;

public class ItemAssociativo
{
    [Key]
    public int ID { get; set; }
    
    [Required]
    public int ItemPaiID { get; set; }

    [ForeignKey(nameof(ItemPaiID))]
    public Item ItemPai { get; set; }

    [Required]
    public int ItemFilhoID { get; set; }

    [ForeignKey(nameof(ItemFilhoID))]
    public Item ItemFilho { get; set; }
}
