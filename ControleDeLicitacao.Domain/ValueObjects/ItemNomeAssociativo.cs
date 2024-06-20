using ControleDeLicitacao.Domain.Entities.Cadastros;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.ValueObjects;

public class ItemNomeAssociativo
{
    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [Required]
    public int ItemID { get; set; }

    [ForeignKey(nameof(ItemID))]
    public Item Item { get; set; }
}
