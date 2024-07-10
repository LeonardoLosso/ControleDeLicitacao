namespace ControleDeLicitacao.App.DTOs.Ata;

public class ReajusteDTO
{
    public int ID { get; set; }
    public int AtaID { get; set; }
    public DateTime Data {  get; set; }
    public List<ItemDeReajusteDTO> Itens { get; set; }
}
