

namespace ControleDeLicitacao.App.DTOs.Baixa;

public class EmpenhoDTO
{
    public int ID { get; set; }
    public int BaixaID { get; set; }
    public string Edital { get; set; }
    public int Unidade { get; set; }
    public int Orgao { get; set; }
    public int Status { get; set; }
    public DateTime? DataEmpenho { get; set; }
    public double Saldo { get; set; }
    public double Valor { get; set; }
    public List<ItemDeEmpenhoDTO> Itens { get; set; }
}
