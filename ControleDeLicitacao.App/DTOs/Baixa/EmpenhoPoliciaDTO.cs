namespace ControleDeLicitacao.App.DTOs.Baixa;

public class EmpenhoPoliciaDTO
{
    public int ID { get; set; }
    public int BaixaID { get; set; }
    public string NumEmpenho { get; set; }
    public string NumNota { get; set; }
    public string Edital { get; set; }
    public DateTime? DataEmpenho { get; set; }
    public double Valor { get; set; }
}
