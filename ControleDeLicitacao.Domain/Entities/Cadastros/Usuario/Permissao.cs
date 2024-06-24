using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;

public class Permissao
{
    [Required]
    public int RecursoId { get; set; }

    [Required]
    public bool PermissaoRecurso {  get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }
}
