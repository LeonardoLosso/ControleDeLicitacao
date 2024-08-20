
namespace ControleDeLicitacao.App.DTOs.Baixa.NotasEmpenhos;

public class NotaDTO
{
    public int ID { get; set; }
    public bool EhPolicia { get; set; } = false;
    public string NumNota { get; set; }
    public string Edital { get; set; }
    public int BaixaID { get; set; }
    public string NumEmpenho { get; set; }
    public int EmpenhoID { get; set; }
    public int Unidade { get; set; }
    public DateTime? DataEmissao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public List<ItemDeNotaDTO> Itens { get; set; }

}
