using ControleDeLicitacao.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;

public class Usuario : IdentityUser<int>
{
    [Required]
    [MaxLength(16)]
    public string UserName {  get; set; }

    [MaxLength(50)]
    public string Nome { get; set; }

    [MaxLength (11)]
    public string CPF { get; set; }
    public int Status { get; set; }

    [MaxLength(11)]
    public string Telefone { get; set; }
    public ICollection<Permissao> Permissoes { get; set; }
    public Endereco Endereco { get; set; }
}
