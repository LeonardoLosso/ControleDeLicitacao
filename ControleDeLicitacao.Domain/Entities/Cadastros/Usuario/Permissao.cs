using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;

public class Permissao
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Tela { get; set; }

    [Required]
    [MaxLength(30)]
    public string NomeRecurso { get; set; }

    [Required]
    public bool PermissaoRecurso {  get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }
}
