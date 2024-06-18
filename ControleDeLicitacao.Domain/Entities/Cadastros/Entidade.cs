using ControleDeLicitacao.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Cadastros;

public class Entidade
{
    [Key]
    [Required]
    public int ID { get; set; }
    public int Status { get; set; }

    [MaxLength(50)]
    public string Nome { get; set; }

    [MaxLength(50)]
    public string Fantasia { get; set; }
    public int Tipo { get; set; }

    [MaxLength(14)]
    [MinLength(11)]
    public string CNPJ { get; set; }

    [MaxLength(9)]
    public string IE { get; set; }

    [MaxLength(11)]
    public string Telefone { get; set; }

    [MaxLength(40)]
    public string Email { get; set; }
    public Endereco Endereco { get; set; }
}