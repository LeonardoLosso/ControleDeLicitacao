using System.ComponentModel.DataAnnotations;

namespace ControleDeLicitacao.Domain.Entities.Documentos.Baixa.NotasEmpenho;

public class EmpenhoPolicia
{
    [Required]
    public int ID { get; set; }

    [Required]
    public int BaixaID { get; set; }

    [MaxLength(10)]
    public string NumEmpenho { get; set; }

    [MaxLength(10)]
    public string NumNota { get; set; }

    [Required, MaxLength(10)]
    public string Edital { get; set; }
    public DateTime? DataEmpenho { get; set; }

    [Required]
    public double Valor { get; set; }

}
