namespace ControleDeLicitacao.App.DTOs.Baixa;

public class EmpenhoSimplificadoDTO
{
    public int ID { get; set; }
    public int BaixaID { get; set; }
    public string Edital { get; set; }
    public string NumEmpenho { get; set; }
    public string Unidade { get; set; }
    public string Orgao { get; set; }
    public int Status { get; set; }
    public DateTime? DataEmpenho { get; set; }
    public double Saldo { get; set; }
    public double Valor { get; set; }
}
