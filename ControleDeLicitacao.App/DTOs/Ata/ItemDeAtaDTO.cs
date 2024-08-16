namespace ControleDeLicitacao.App.DTOs.Ata;

public class ItemDeAtaDTO
{
    public int ID { get; set; }
    public int AtaID { get; set; }
    public string Nome { get; set; }
    public string Unidade { get; set; }
    public double QtdeLicitada { get; set; }
    public double ValorUnitario { get; set; }
    public double ValorLicitado { get; set; }
    public double Desconto { get; set; }
}
