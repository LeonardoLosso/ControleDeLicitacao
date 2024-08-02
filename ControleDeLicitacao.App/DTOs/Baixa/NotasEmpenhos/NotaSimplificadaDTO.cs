
namespace ControleDeLicitacao.App.DTOs.Baixa.NotasEmpenhos;

public class NotaSimplificadaDTO
{
    public int ID { get; set; }
    public string NumNota { get; set; }
    public string NumEmpenho { get; set; }
    public int EmpenhoID { get; set; }
    public string Unidade { get; set; }
    public DateTime? DataEmissao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public double ValorEntregue { get; set; }
}
