namespace ControleDeLicitacao.App.DTOs.Baixa.NotasEmpenhos;

public class ItemDeNotaDTO
{
    public int ID { get; set; }
    public int NotaID { get; set; }
    public int EmpenhoID { get; set; }
    public string Unidade { get; set; }
    public double Quantidade { get; set; }
    public double ValorUnitario { get; set; }
    public double ValorTotal { get; set; }

}
