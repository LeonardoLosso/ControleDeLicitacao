using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;

public class Nota
{
    [Key]
    public int ID { get; set; }

    public string Observacao { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string NumNota { get; set; }

    [Required]
    [MaxLength(10)]
    public string Edital { get; set; }

    [Required]
    public int BaixaID { get; set; }

    [Required]
    [MaxLength(10)]
    public string NumEmpenho { get; set; }

    [Required]
    public int EmpenhoID { get; set; }

    [Required]
    public int Unidade { get; set; }

    public DateTime? DataEmissao { get; set; }

    public DateTime? DataEntrega { get; set; }

    public ICollection<ItemDeNota> Itens { get; set; }

}
