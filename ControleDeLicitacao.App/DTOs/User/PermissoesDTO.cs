namespace ControleDeLicitacao.App.DTOs.User;

public class PermissoesDTO
{
    public int Id { get; set; }

    public string Tela {  get; set; }
    public List<RecursosDTO> Recursos {  get; set; }
}

public class RecursosDTO
{
    public int Id { get; set; }
    public string NomeRecurso { get; set; }
    public bool PermissaoRecurso { get; set; } = false;

}
