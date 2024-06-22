using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.App.DTOs.User;

public class LoginUsuarioDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
