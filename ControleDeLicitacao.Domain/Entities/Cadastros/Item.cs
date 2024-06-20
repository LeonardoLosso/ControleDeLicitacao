using ControleDeLicitacao.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Cadastros;

public class Item
{
    [Key]
    public int Id { get; set; }
    public int Status { get; set; }

    public bool EhCesta { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [MaxLength(10)]
    public string UnidadePrimaria { get; set; }

    [MaxLength(10)]
    public string UnidadeSecundaria { get; set; }

    public ICollection<ItemAssociativo> ListaItens { get; set; }

    public ICollection<ItemNomeAssociativo> ListaNomes { get; set; }
}
