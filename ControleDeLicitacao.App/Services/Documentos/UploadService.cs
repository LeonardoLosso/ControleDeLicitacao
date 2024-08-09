using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.App.DTOs.Entidades;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Documentos.Ata;
using ControleDeLicitacao.App.Upload.Services;
using ControleDeLicitacao.Common.Enum;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using UglyToad.PdfPig;

namespace ControleDeLicitacao.App.Services.Documentos;

public class UploadService
{
    private readonly RequestService _requestService;
    private readonly AtaService _ataService;
    private readonly EntidadeService _entidadeService;
    private readonly ItemService _itemService;
    public UploadService
        (RequestService requestService, AtaService ataService,
        EntidadeService entidadeService, ItemService itemService)
    {
        _requestService = requestService;
        _ataService = ataService;
        _entidadeService = entidadeService;
        _itemService = itemService;
    }
    public async Task<AtaDTO> UploadAta(IFormFile file)
    {
        var documento = await PdfToString(file);

        if (string.IsNullOrWhiteSpace(documento)) throw new GenericException("Não foi possivel ler o documento", 501);

        var template = GeraTemplate();

        var response = await _requestService.BuildRequest(documento, template);

        var retorno = response.Candidates[0].Content.Parts[0].Text;

        var ataExtraida = await JsonToAtaLicitacao(retorno);

        return await _ataService.Adicionar(ataExtraida);
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
            "preciso que extraia pra mim informações de um texto. " +
            "Você ira encontrar informações da entidade licitante, empresa contratada, e itens." +
            "A respeito dos itens, preciso apenas daqueles itens que contenham quantidade, valor unitario e valor total. ";

        string reforco =
            "Quanto ao nome do item extraia apenas o nome dele como: " +
            "'abobora', 'morango', 'limão taiti', substantivos proprios " +
            "(ignore coisas como 'in natura', 'fruta' e as descrições) ";

        string condicional =
            "Caso um item repita, some suas quantidades de valores totais. ";

        string modeloDeRetorno =
            "O objeto de retorno deve atender esse formato: " +
            "\"DocumentoExtraido\": { \"NumAta\": string,\"DataAta\": date," +
            "\"Licitante\": {\"CNPJ\": string,\"Nome\": string,\"CEP\": string,\"Estado\": string,\"Logradouro\": string, \"Numero\": string,\"Email\": string, \"Telefone\": string,\"Cidade\": string}," +
            "\"Empresa\": { \"CNPJ\": string,\"Nome\": string,\"CEP\": string, \"Estado\": string, \"Logradouro\": string, \"Numero\": string, \"Email\": string, \"Telefone\": string,\"Cidade\": string}," +
            "\"Itens\":[{\"Nome\": string, \"Unidade\": string, \"Quantidade\": number, \"ValorUnitario\": number, \"ValorTotal\": number}]}";

        string template = (
            cabecalho +
            reforco +
            condicional +
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
    internal class EntidadeDeRetorno
    {
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }
    internal class DocumentoExtraido
    {
        public string NumAta { get; set; }
        public DateTime? DataAta { get; set; }
        public EntidadeDeRetorno Licitante { get; set; }
        public EntidadeDeRetorno Empresa { get; set; }
        public List<ItemDeRetorno> Itens { get; set; }
    }
    internal class Retorno
    {
        public DocumentoExtraido DocumentoExtraido { get; set; }
    }

    private async Task<AtaDTO?> JsonToAtaLicitacao(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var lista = JsonSerializer.Deserialize<Retorno>(json, options);

        if (lista is null) return null;

        var ata = new AtaDTO();
        ata.ID = 0;
        ata.Status = 1;
        ata.Edital = lista.DocumentoExtraido.NumAta;
        ata.TotalReajustes = 0;
        if (lista.DocumentoExtraido.DataAta is not null)
        {
            ata.DataAta = lista.DocumentoExtraido.DataAta;
            ata.Vigencia = ata.DataAta?.AddYears(1);
        }
        try
        {
            var itens = await RetornaItens(lista.DocumentoExtraido.Itens);
            ata.TotalLicitado = itens.Sum(i => i.ValorTotal);
            ata.Itens = itens;
        }
        catch { throw new GenericException("Erro ao extrair Itens", 501); }

        try
        {
            var licitante = await RetornaEntidade(lista.DocumentoExtraido.Licitante, 2);
            ata.Orgao = licitante.ID;
            ata.Unidade = licitante.Tipo;
        }
        catch { throw new GenericException("Erro ao extrair Órgão", 501); }

        try
        {
            var empresa = await RetornaEntidade(lista.DocumentoExtraido.Empresa);
            ata.Empresa = empresa.ID;
        }
        catch { throw new GenericException("Erro ao extrair Empresa", 501); }

        return ata;
    }

    private async Task<List<ItemDeAtaDTO>> RetornaItens(List<ItemDeRetorno> itens)
    {
        var itensAta = new List<ItemDeAtaDTO>();

        foreach (var item in itens)
        {
            var itemAta = new ItemDeAtaDTO()
            {
                ID = 0,
                AtaID = 0,
                Desconto = 0,
                Nome = item.Nome ?? "",
                Quantidade = item.Quantidade,
                Unidade = item.Unidade ?? "",
                ValorUnitario = item.ValorUnitario,
                ValorTotal = item.ValorTotal
            };

            itensAta.Add(itemAta);
        }

        itensAta = await _itemService.PreencherExtracao(itensAta);

        return itensAta;
    }

    private async Task<EntidadeDTO> RetornaEntidade(EntidadeDeRetorno retorno, int tipo = 1)
    {
        var entidade = new EntidadeDTO();
        entidade.CNPJ = retorno.CNPJ ?? "";
        entidade.Telefone = retorno.Telefone ?? "";
        entidade.Email = retorno.Email ?? "";
        entidade.Nome = retorno.Nome ?? "";
        entidade.Fantasia = retorno.Nome ?? "";
        entidade.Endereco = new EnderecoDTO();
        entidade.Endereco.Logradouro = retorno.Logradouro ?? "";
        entidade.Endereco.Numero = retorno.Numero ?? "";
        entidade.Endereco.Cidade = retorno.Cidade ?? "";
        entidade.Endereco.UF = RetornaUF(retorno.Estado);
        entidade.Tipo = entidade.Tipo == 1 ? 1 : RetornaTipo(entidade.Nome);
        entidade.ID = 0;
        entidade.IE = "";
        entidade.Status = 1;

        entidade = await _entidadeService.BuscaEntidadesPorCNPJ(entidade);

        return entidade;
    }

    private int RetornaTipo(string nome)
    {
        var nomeFormatado = nome.ToUpper();

        string[] tipoPref = { "PREFEITURA", "MUNICIPIO", "MUNICÍPIO", "CIDADE" };

        bool prefeitura = tipoPref.Any(tipo => nomeFormatado.Contains(tipo));

        if (prefeitura) return 2;

        return 3;
    }

    private string RetornaUF(string estado)
    {
        if (estado.Length == 2)
            return estado;

        return EstadosBrasil.ObterSigla(estado);
    }
}

