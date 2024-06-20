using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.App.DTOs.Itens;

public class ItemDTO
{
    public int Id { get; set; }
    public int Status { get; set; }

    public bool EhCesta { get; set; }

    [Required]
    public string Nome { get; set; }

    [StringLength(10)]
    public string UnidadePrimaria { get; set; }

    [StringLength(10)]
    public string UnidadeSecundaria { get; set; }

    public List<ItemSimplificadoDTO>? ListaItens { get; set; }

    public List<string> ListaNomes { get; set; }
}
