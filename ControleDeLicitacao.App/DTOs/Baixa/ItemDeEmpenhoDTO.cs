namespace ControleDeLicitacao.App.DTOs.Baixa;

public class ItemDeEmpenhoDTO
{
    public int ID { get; set; }
    public int EmpenhoID { get; set; }
    public int BaixaID { get; set; }
    public string Nome { get; set; }
    public string Unidade { get; set; }
    public double QtdeEmpenhada { get; set; }
    public double QtdeEntregue { get; set; }
    public double QtdeAEntregar { get; set; }
    public double ValorEntregue { get; set; }
    public double ValorUnitario { get; set; }
    public double Total { get; set; }
}
