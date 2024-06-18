namespace ControleDeLicitacao.App.DTOs;

public class ListagemDTO<T>
{
    public int Page { get; set; }
    public int TotalPage { get; set; }
    public int TotalItems {  get; set; }
    public List<T> Lista {  get; set; }

    public void CalcularTotalPage()
    {
        const int pageSize = 15;
        TotalPage = (int)Math.Ceiling(TotalItems / (double)pageSize);
        if (TotalPage == 0)
        {
            TotalPage = 1;
        }
    }
}
