namespace ControleDeLicitacao.App.DTOs.Ata;

public class AtaDTO
{
    public int ID { get; set; }
    public string Edital { get; set; }
    public int Status { get; set; }
    public int Unidade { get; set; }
    public int Empresa { get; set; }
    public int Orgao { get; set; }
    public DateTime? DataLicitacao { get; set; }
    public DateTime? DataAta { get; set; }
    public DateTime? Vigencia { get; set; }
    public List<ItemDeAtaDTO> Itens { get; set; }
    public int TotalReajustes { get; set; }
}
