using ControleDeLicitacao.App.Upload.Models;
using ControleDeLicitacao.App.Upload.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using UglyToad.PdfPig;

namespace ControleDeLicitacao.App.Services.Documentos;

public class UploadService
{
    private readonly RequestService _requestService;
    public UploadService(RequestService requestService)
    {
        _requestService = requestService;
    }
    public async Task Upload(IFormFile file)
    {
        //APARENTEMENTE NÃO PARECE UMA BOA IDEIA MANDAR O ARQUIVO DIRETAMENTE, TENTAREMOS COMO UM TEXTO DE FATO 

        var documento = await PdfToString(file);

        if (string.IsNullOrWhiteSpace(documento)) return;

        var template = GeraTemplate();
            
        var response = await _requestService.BuildRequest(documento, template);

        var itemJson = response.Candidates[0].Content.Parts[0].Text;

        var item = JsonToItemDeRetorno(itemJson);
    }

    private async Task<string> PdfToString(IFormFile file)
    {
        var filePath = Path.GetTempFileName();
        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        string fileContent;
        using (var pdf = PdfDocument.Open(filePath))
        {
            var text = new System.Text.StringBuilder();
            foreach (var page in pdf.GetPages())
            {
                text.Append(page.Text);
            }
            fileContent = text.ToString();
        }

        System.IO.File.Delete(filePath);

        return fileContent;
    }


    //------------------------------------[GEMINI]--------------------------------------

    private string GeraTemplate()
    {
        string cabecalho = 
            "extraia do texto apenas os itens que contenham as informações: " +
            "Nome, Unidade, Quantidade, ValorUnitario e ValorTotal ";

        string reforco = "o item PRECISA conter as informações de valores para ser um item válido ";

        string condicional = 
            "Caso não encontre as informações a partir das descrições " +
            "pode usar seus proprios requisitos de análise ";

        string padroes =
            "Os itens serão encontrados no seguinte padrão: " +
            "Nome string (o nome será encontrado no campo PRODUTO, desejo apenas o nome do item, desconsiderando informações desnecessarias como 'in natura' e a sua descrição)" +
            "Unidade string (campo UNID) " +
            "Quantidade number (campo QTDADE)" +
            "ValorUnitario number (campo VALOR UNITÁRIO) ValorTotal number ";

        string modeloDeRetorno = 
            "O retorno deve obedecer a esse padrão: " +
            "\"itens\":[{\"nome\": string, \"unidade\": string, \"quantidade\": number, \"valorUnitario\": number, \"valorTotal\": number}]";

        string template = (
            cabecalho +
            reforco +
            condicional +
            padroes +
            modeloDeRetorno);
        return template;
    }
    internal class ItemDeRetorno
    {
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public double Quantidade { get; set; }
        public double ValorUnitario { get; set; }
        public double ValorTotal { get; set; }
    }
    internal class ListaRetorno
    {
        public List<ItemDeRetorno> Itens { get; set; }
    }
    //private void TrataReornoDeItem()
    //{

    //}
    private List<ItemDeRetorno> JsonToItemDeRetorno(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var lista = JsonSerializer.Deserialize<ListaRetorno>(json, options);

        return lista.Itens;
    }
}

