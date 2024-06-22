using ControleDeLicitacao.App.DTOs.Entidades;
using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.App.DTOs.User;

public class UsuarioDTO
{
    public int Id { get; set; }

    [DataType(DataType.Password)]
    public string Password { get; set; }


    [Compare("Password")]
    public string RePassword { get; set; }

    [Required]
    public string UserName { get; set; }

    public string Nome { get; set; }
    public string CPF { get; set; }
    public int Status { get; set; }
    public string Telefone { get; set; }
    public string Email { get; set; }
    public EnderecoDTO Endereco { get; set; }

    public List<PermissoesDTO> Permissoes { get; set; }
}
