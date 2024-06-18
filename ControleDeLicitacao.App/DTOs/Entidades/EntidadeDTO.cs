using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.App.DTOs.Entidades;

public class EntidadeDTO
{
    public int ID { get; set; }
    public int Status { get; set; }

    [Required(ErrorMessage = "O nome do cadastro é obrigatório!")]
    public string Nome { get; set; }
    public string Fantasia { get; set; }

    [Required(ErrorMessage = "O tipo de cadastro é obrigatório!")]
    public int Tipo { get; set; }
    public string CNPJ { get; set; }
    public string IE { get; set; }
    public string Telefone { get; set; }
    public string Email { get; set; }
    public EnderecoDTO Endereco { get; set; }
}
