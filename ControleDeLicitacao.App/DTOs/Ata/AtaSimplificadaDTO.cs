namespace ControleDeLicitacao.App.DTOs.Ata;

public class AtaSimplificadaDTO
{
    public int ID { get; set; }
    public string Edital { get; set; }
    public int Status { get; set; }
    public int Unidade { get; set; }
    public string Empresa { get; set; }
    public string Orgao { get; set; }
    public DateTime? DataLicitacao { get; set; }
    public DateTime? DataAta { get; set; }
    public double TotalLicitado { get; set; }
}
