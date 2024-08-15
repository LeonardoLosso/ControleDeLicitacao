namespace ControleDeLicitacao.App.DTOs.Baixa;

public class BaixaPoliciaDTO
{
    public int ID { get; set; }
    public int Status { get; set; }
    public string Edital { get; set; }
    public int Empresa { get; set; }
    public int Orgao { get; set; }
    public DateTime? DataLicitacao { get; set; }
    public DateTime? DataAta { get; set; }
    public DateTime? Vigencia { get; set; }
    public double ValorLicitado { get; set; }
    public double ValorEmpenhado { get; set; }
    public double ValorEntregue { get; set; }
    public List<EmpenhoPoliciaDTO> Empenhos { get; set; }
}
