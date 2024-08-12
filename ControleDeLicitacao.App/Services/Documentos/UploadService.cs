using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.DTOs.Entidades;
using ControleDeLicitacao.App.DTOs.Itens;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Documentos.Ata;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Upload.Services;
using ControleDeLicitacao.Common.Enum;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace ControleDeLicitacao.App.Services.Documentos;

public class UploadService
{
    private readonly RequestService _requestService;
    private readonly AtaService _ataService;
    private readonly BaixaService _baixaService;
    private readonly EntidadeService _entidadeService;
    private readonly ItemService _itemService;
    public UploadService
        (RequestService requestService, AtaService ataService,
        EntidadeService entidadeService, ItemService itemService, BaixaService baixaService)
    {
        _requestService = requestService;
        _ataService = ataService;
        _entidadeService = entidadeService;
        _itemService = itemService;
        _baixaService = baixaService;
    }
    public async Task<AtaDTO> UploadAta(IFormFile file)
    {
        var documento = await PdfToString(file);
        int contagem = Regex.Matches(documento, "NOTA DE EMPENHO", RegexOptions.IgnoreCase).Count;

        if (contagem > 1) throw new GenericException("O Documento deve ser uma ATA", 501);

        if (string.IsNullOrWhiteSpace(documento)) throw new GenericException("Não foi possivel ler o documento", 501);

        var template = GeraTemplateAta();

        var response = await _requestService.BuildRequest(documento, template);

        string retorno = response.Candidates[0].Content.Parts[0].Text;

        var ataExtraida = await JsonToAtaLicitacao(retorno);

        ataExtraida = await _ataService.Adicionar(ataExtraida);

        var baixa = await _baixaService.Adicionar(ataExtraida.ID);

        return ataExtraida;
    }
    public async Task<EmpenhoDTO> UploadEmpenho(IFormFile file, int idBaixa)
    {
        var documento = await PdfToString(file);
        int contagem = Regex.Matches(documento, "ATA DE REGISTRO DE PREÇOS", RegexOptions.IgnoreCase).Count;

        if (contagem > 0) throw new GenericException("O Documento deve ser um EMPENHO", 501);

        if (string.IsNullOrWhiteSpace(documento)) throw new GenericException("Não foi possivel ler o documento", 501);

        var template = GeraTemplateEmpenho();

        var response = await _requestService.BuildRequest(documento, template);

        string retorno = response.Candidates[0].Content.Parts[0].Text;

        var empenhoExtraido = await JsonToEmpenho(retorno, idBaixa);

        //empenhoExtraido = await _ataService.Adicionar(empenhoExtraido);

        return empenhoExtraido;
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
        if (string.IsNullOrWhiteSpace(fileContent)) throw new GenericException("O PFD contêm apenas imagens", 501);
        return fileContent;
    }


    //------------------------------------[GEMINI]--------------------------------------

    private string GeraTemplateAta()
    {
        string cabecalho =
            "preciso que extraia pra mim informações de um texto. " +
            "Você ira encontrar informações da entidade licitante (será sempre uma prefeitura ou batalhão de policia), empresa contratada, e itens." +
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
    private string GeraTemplateEmpenho()
    {
        string cabecalho =
            "preciso que extraia pra mim informações de um texto. " +
            "Você ira encontrar informações da entidade licitante (será sempre umórgão publico que está solicitando a entrega), " +
            "valor empenhado e os itens (se houver)." +
            "A respeito dos itens, preciso apenas daqueles itens que contenham quantidade, valor unitario e valor total. ";

        string reforco =
            "Quanto ao nome do item extraia apenas o nome dele como: " +
            "'abobora', 'morango', 'limão taiti', substantivos proprios " +
            "(ignore coisas como 'in natura', 'fruta' e as descrições) ";

        string condicional =
            "Caso um item repita, some suas quantidades de valores totais. " +
            "Se o não houverem itens, retorne apenas um array vazio []";

        string modeloDeRetorno =
            "O objeto de retorno deve atender esse formato: " +
            "\"EmpenhoExtraido\": { \"NumEmpenho\": string,\"DataEmpenho\": date,\"ValorEmpenhado\": number," +
            "\"Licitante\": {\"CNPJ\": string,\"Nome\": string,\"CEP\": string,\"Estado\": string,\"Logradouro\": string, \"Numero\": string,\"Email\": string, \"Telefone\": string,\"Cidade\": string}," +
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
    internal class EmpenhoExtraido
    {
        public string NumEmpenho { get; set; }
        public DateTime? DataEmpenho { get; set; }
        public double ValorEmpenhado { get; set; }
        public EntidadeDeRetorno Licitante { get; set; }
        public List<ItemDeRetorno> Itens { get; set; }
    }
    internal class RetornoEmpenho
    {
        public EmpenhoExtraido EmpenhoExtraido { get; set; }
    }

    private async Task<AtaDTO> JsonToAtaLicitacao(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        var lista = new Retorno();
        try
        {
            lista = JsonSerializer.Deserialize<Retorno>(json, options);
        }
        catch { throw new GenericException("Falha na extração formato ATA", 501); }

        if (lista is null) throw new GenericException("Falha na extração formato ATA", 501);

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
            ata.Orgao = licitante is not null ? licitante.ID : 0;
            ata.Unidade = licitante is not null ? licitante.Tipo : 0;
        }
        catch { throw new GenericException("Erro ao extrair Órgão", 501); }

        try
        {
            var empresa = await RetornaEntidade(lista.DocumentoExtraido.Empresa);
            ata.Empresa = empresa is not null ? empresa.ID : 0;
        }
        catch { throw new GenericException("Erro ao extrair Empresa", 501); }

        return ata;
    }

    private async Task<EmpenhoDTO> JsonToEmpenho(string json, int idBaixa)
    {
        var baixa = await _baixaService.ObterPorID(idBaixa);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        var lista = new RetornoEmpenho();
        try
        {
            lista = JsonSerializer.Deserialize<RetornoEmpenho>(json, options);
        }
        catch { throw new GenericException("Falha na extração formato EMPENHO", 501); }

        if (lista is null) throw new GenericException("Falha na extração formato EMPENHO", 501);

        var empenho = new EmpenhoDTO();

        empenho.ID = 0;
        empenho.BaixaID = baixa.ID;
        empenho.Edital = baixa.Edital;
        empenho.Orgao = baixa.Orgao;
        empenho.Unidade = baixa.Orgao;
        empenho.Status = 1;

        empenho.NumEmpenho = lista.EmpenhoExtraido.NumEmpenho;
        empenho.Valor = lista.EmpenhoExtraido.ValorEmpenhado;
        empenho.Saldo = empenho.Valor;

        if (lista.EmpenhoExtraido.DataEmpenho is not null)
        {
            empenho.DataEmpenho = lista.EmpenhoExtraido.DataEmpenho;
        }
        try
        {
            var itens = await RetornaItensEmpenho(lista.EmpenhoExtraido.Itens, baixa.Itens);
            empenho.Itens = itens;
        }
        catch { throw new GenericException("Erro ao extrair Itens", 501); }

        return empenho;
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
    private List<ItemDeEmpenhoDTO> RetornaItensEmpenho(List<ItemDeRetorno> itens, List<ItemDeBaixaDTO> itensBaixa)
    {
        var itensEmpenho = new List<ItemDeEmpenhoDTO>();

        foreach (var item in itens)
        {
            var itemBaixa = BuscarItemBaixa(item, itensBaixa);
            var itemEmpenho = new ItemDeEmpenhoDTO()
            {
                ID = 0,
                EmpenhoID = ,
                Desconto = 0,
                Nome = item.Nome ?? "",
                Quantidade = item.Quantidade,
                Unidade = item.Unidade ?? "",
                ValorUnitario = item.ValorUnitario,
                ValorTotal = item.ValorTotal
            };

            itensEmpenho.Add(itemEmpenho);
        }

        return itensEmpenho;
    }

    private ItemDeBaixaDTO BuscarItemBaixa(ItemDeRetorno item, List<ItemDeBaixaDTO> itensBaixa)
    {
        var itemExtract = ObterParaExtracao(item.Nome, itensBaixa);
        var itemDeBaixa = new ItemDeBaixaDTO()
        {
            ID = itemExtract.ID,


        };
        item.Unidade =
            !item.Unidade.Contains(" ") ?
                item.Unidade : item.Unidade.Split(' ')[0];
        item.Nome = itemExtract.Nome;

        return itensBaixa;
    }

    public ItemDeBaixaDTO ObterParaExtracao(string nome, List<ItemDeBaixaDTO> itensBaixa)
    {
        nome = nome.Trim();

        var busca = itensBaixa.Where(w => w.Nome == nome).FirstOrDefault();

        if (busca is not null) return busca;

        if (nome.Contains("/"))
            return ObterComTratamentoDeNome(nome, '/', itensBaixa);

        if (nome.Contains(" "))
            return ObterComTratamentoDeNome(nome, ' ', itensBaixa);


        return new ItemDeBaixaDTO()
        {
            ID = 0,
            Unidade = " ",
            Nome = $@"NÃO ENCONTRADO ({nome})"
        };
    }
    private ItemDeBaixaDTO ObterComTratamentoDeNome(string nome, char separador, List<ItemDeBaixaDTO> itensBaixa)
    {
        var nomeComposto = nome.Split(separador).ToList();
        var nomesDistinct = nomeComposto.Distinct().ToList();

        if (separador == '/')
        {
            var aux = new List<string>();
            foreach (var name in nomeComposto)
            {
                aux.AddRange(name.Trim().Split(' '));
            }
            nomeComposto.Clear();
            nomeComposto = aux;

            nomesDistinct.Clear();
            nomesDistinct = nomeComposto.Distinct().ToList();


            for (var i = 1; i < nomesDistinct.Count; i++)
            {
                var item = itensBaixa
                    .Where(w => w.Nome
                    .Contains(
                        string.Concat(nomesDistinct[0], ' ', nomesDistinct[i])
                    )).ToList();

                if (item.Count == 1)
                    return item.First();
            }
        }

        var busca = itensBaixa
                    .Where(w => w.Nome
                    .Contains(nomesDistinct[0]))
                    .ToList();

        if (busca.Count == 1)
            return busca.First();

        return new ItemDeBaixaDTO();
    }
    private async Task<EntidadeDTO?> RetornaEntidade(EntidadeDeRetorno retorno, int tipo = 1)
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

        if (string.IsNullOrWhiteSpace(entidade.CNPJ)) return null;

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
        if (string.IsNullOrWhiteSpace(estado)) return "";
        if (estado.Length == 2)
            return estado;

        return EstadosBrasil.ObterSigla(estado);
    }
}

