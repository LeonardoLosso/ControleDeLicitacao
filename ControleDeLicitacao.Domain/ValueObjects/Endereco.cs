using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.ValueObjects;

public class Endereco
{
    [MaxLength(8)]
    public string CEP { get; set; }

    [MaxLength(30)]
    public string Cidade { get; set; }

    [MaxLength(2)]
    public string UF { get; set; }

    [MaxLength(30)]
    public string Bairro { get; set; }

    [MaxLength(30)]
    public string Logradouro { get; set; }

    [MaxLength(10)]
    public string Numero { get; set; }

    [MaxLength(30)]
    public string Complemento { get; set; }
}
