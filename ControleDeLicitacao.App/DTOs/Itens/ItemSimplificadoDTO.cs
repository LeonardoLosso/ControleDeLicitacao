using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.App.DTOs.Itens;

public class ItemSimplificadoDTO
{

    public int ID {get;set;}
    public int Status {get;set;}
    public bool EhCesta {get;set;}
    public string Nome {get;set;}

    [StringLength(10)]
    public string UnidadePrimaria { get; set; }

    [StringLength(10)]
    public string UnidadeSecundaria {get;set;}
}
