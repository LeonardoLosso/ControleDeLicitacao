using ControleDeLicitacao.App.DTOs.Ata;
using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.DTOs.Entidades;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using ControleDeLicitacao.App.Upload.Services;
using ControleDeLicitacao.Common;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace ControleDeLicitacao.App.Services.Documentos;

public class UploadService
{
    private readonly RequestService _requestService;
    private readonly BaixaService _baixaService;
    private readonly EmpenhoService _empenhoService;
    private readonly EntidadeService _entidadeService;
    private readonly ItemService _itemService;
    public UploadService
        (RequestService requestService,
        EntidadeService entidadeService, ItemService itemService,
        BaixaService baixaService, EmpenhoService empenhoService)
    {
        _requestService = requestService;
        _entidadeService = entidadeService;
        _itemService = itemService;
        _baixaService = baixaService;
        _empenhoService = empenhoService;
    }
    public async Task<AtaDTO> UploadAta(IFormFile file)
    {
        var documento = await PdfToString(file);
        int contagem = Regex.Matches(documento.Substring(0, 200), "NOTA DE EMPENHO", RegexOptions.IgnoreCase).Count;

        if (contagem > 0) throw new GenericException("O Documento deve ser uma ATA", 501);

        if (string.IsNullOrWhiteSpace(documento)) throw new GenericException("Não foi possivel ler o documento", 501);

        string cnpj = ExtraiCNPJ(documento);
        var template = GeraTemplateAta(cnpj);

        var response = await _requestService.BuildRequest(documento, template);

        string retorno = response.Candidates[0].Content.Parts[0].Text;

        var ataExtraida = await JsonToAtaLicitacao(retorno);

        ataExtraida = await _baixaService.Adicionar(ataExtraida);


        return ataExtraida;
    }

    private string ExtraiCNPJ(string documento)
    {
        var cnpjs = _entidadeService.BuscarEmpresas();

        var cnpjsEncontrados = VerificarEObterCnpjsNoTexto(documento, cnpjs);

        if (!cnpjsEncontrados.Any()) throw new GenericException("Cnpj cadastrado não encontrado no documento", 501);

        return cnpjsEncontrados.First();
    }
    public static List<string> VerificarEObterCnpjsNoTexto(string texto, List<string> listaDeCnpjs)
    {
        List<string> cnpjsEncontrados = new List<string>();

        // Expressão regular para encontrar CNPJs no texto
        var cnpjPattern = @"\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}";
        var matches = Regex.Matches(texto, cnpjPattern);

        foreach (Match match in matches)
        {
            string cnpjEncontrado = match.Value;

            // Remove caracteres especiais do CNPJ encontrado para compará-lo
            string cnpjSemFormatacao = Regex.Replace(cnpjEncontrado, @"\D", "");

            foreach (var cnpj in listaDeCnpjs)
            {
                string cnpjListaSemFormatacao = Regex.Replace(cnpj, @"\D", "");

                if (cnpjSemFormatacao == cnpjListaSemFormatacao)
                {
                    cnpjsEncontrados.Add(cnpjEncontrado);
                }
            }
        }

        return cnpjsEncontrados;
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

        empenhoExtraido = await _empenhoService.AdicionarImportacao(empenhoExtraido);

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
    private string GeraTemplateAta(string cnpj)
    {
        string cabecalho =
            $"preciso que extraia pra mim informações de um texto APENAS sobre a empresa com o cnpj: {cnpj}. " +
            "Ignore completamente itens e dados de outras empresas" +
            "Você ira encontrar informações da entidade licitante (será sempre uma prefeitura ou batalhão de policia), empresa contratada, data da licitação e itens. " +
            "Tipo: Empresa = 1, Prefeitura = 2, Policia/secretaria de segurança = 3. Caso não entre no meu padrão, defina o tipo como 2" +
            "A respeito dos itens, preciso apenas daqueles itens que contenham quantidade e valor unitario. " ;

        string reforco =
            "Quanto ao nome do item extraia apenas o nome dele como: " +
            "'abobora', 'morango', 'limão', substantivos proprios, " +
            "se conter tipo retorne com o tipo se fizer parte do nome, por exemplo: maçã gala, banana nanica... etc " +
            "preciso do nome do item" +
            "(ignore coisas como 'in natura', 'fruta' e as descrições) ";

        string modeloDeRetorno =
            "O objeto de retorno deve atender esse formato: " +
            "\"DocumentoExtraido\": { \"NumAta\": string,\"DataAta\": date," +

            "\"Licitante\": {\"CNPJ\": string,\"Nome\": string,\"Tipo\": number}," +

            "\"Empresa\": {\"CNPJ\": string,\"Nome\": string,\"Tipo\": number}," +

            "\"Itens\":[{\"Nome\": string, \"Quantidade\": number, \"ValorUnitario\": number}]}";

        string template = (
            cabecalho +
            reforco +
            modeloDeRetorno);

        return template;

    }
    private string GeraTemplateEmpenho()
    {
        string cabecalho =
            "preciso que extraia pra mim informações de um texto. " +
            "Você ira encontrar informações da entidade licitante (será sempre um órgão publico que está solicitando a entrega), " +
            "valor empenhado, data do empenho e os itens (se houver)." +
            "A respeito dos itens, preciso apenas daqueles itens que contenham quantidade e valor unitario. " +
            "A respeito da data preciso dela no formato YYYY-MM-DD";

        string reforco =
            "Quanto ao nome do item extraia apenas o nome dele como: " +
            "'abobora', 'morango', 'limão taiti', substantivos proprios " +
            "(ignore coisas como 'in natura', 'fruta' e as descrições) ";

        string condicional =
            "Se o não houverem itens, retorne apenas um array vazio []";

        string modeloDeRetorno =
            "O objeto de retorno deve atender esse formato: " +
            "\"EmpenhoExtraido\": { \"NumEmpenho\": string,\"DataEmpenho\": date,\"ValorEmpenhado\": number," +
            "\"Licitante\": {\"CNPJ\": string,\"Nome\": string}," +
            "\"Itens\":[{\"Nome\": string, \"Quantidade\": number, \"ValorUnitario\": number}]}";

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
        public double Quantidade { get; set; }
        public double ValorUnitario { get; set; }
    }
    internal class EntidadeDeRetorno
    {
        public string? CNPJ { get; set; }
        public string? Nome { get; set; }
        public int? Tipo { get; set; }
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
        catch (Exception ex)
        {
            throw new GenericException("Falha na extração formato ATA", 501);
        }

        if (lista is null) throw new GenericException("Falha na extração formato ATA", 501);

        var edital = lista.DocumentoExtraido.NumAta.RemoveSpaces();
        if (!string.IsNullOrEmpty(edital))
            edital = edital.Length > 10? edital.Substring(edital.Length - 10) : edital;

        var ata = new AtaDTO();
        ata.ID = 0;
        ata.Status = 1;
        ata.Edital = edital;
        ata.TotalReajustes = 0;
        if (lista.DocumentoExtraido.DataAta is not null)
        {
            ata.DataAta = lista.DocumentoExtraido.DataAta;
            ata.Vigencia = ata.DataAta?.AddYears(1);
        }
        try
        {
            var itens = await RetornaItens(lista.DocumentoExtraido.Itens);
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

        if (baixa is null) throw new GenericException("Baixa não encontrada", 501);

        if (baixa.Status != 1) throw new GenericException("Baixa está inativa", 501);

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

        empenho.NumEmpenho = lista.EmpenhoExtraido.NumEmpenho.RemoveSpaces();
        empenho.Valor = lista.EmpenhoExtraido.ValorEmpenhado;
        empenho.Saldo = empenho.Valor;

        if (lista.EmpenhoExtraido.DataEmpenho is not null)
        {
            empenho.DataEmpenho = lista.EmpenhoExtraido.DataEmpenho;
        }
        try
        {
            var itens = RetornaItensEmpenho(lista.EmpenhoExtraido.Itens, baixa.Itens);
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
                QtdeLicitada = item.Quantidade,
                Unidade = "",
                ValorUnitario = item.ValorUnitario,
                ValorLicitado = item.ValorUnitario * item.Quantidade
            };

            itensAta.Add(itemAta);
        }

        itensAta = await _itemService.PreencherExtracao(itensAta);

        return itensAta;
    }
    private List<ItemDeEmpenhoDTO> RetornaItensEmpenho(List<ItemDeRetorno> itens, List<ItemDeBaixaDTO> itensBaixa)
    {
        var itensEmpenho = new List<ItemDeEmpenhoDTO>();

        var idAux = 100000;
        foreach (var item in itens)
        {
            var itemBaixa = BuscarItemBaixa(item, itensBaixa);
            var itemEmpenho = new ItemDeEmpenhoDTO()
            {
                ID = itemBaixa.ID != 0 ? itemBaixa.ID : idAux++,
                EmpenhoID = 0,
                Nome = itemBaixa.Nome ?? "",
                QtdeEmpenhada = item.Quantidade,
                QtdeAEntregar = item.Quantidade,
                QtdeEntregue = 0,
                BaixaID = itemBaixa.BaixaID,

                Unidade = itemBaixa is null ?
                    "" : itemBaixa.Unidade,

                ValorUnitario = itemBaixa is null ? 
                    item.ValorUnitario : itemBaixa.ValorUnitario,

                Total = itemBaixa.ValorUnitario * item.Quantidade,
                ItemDeBaixa = itemBaixa.ID != 0 ? true : false,
                ValorEntregue = 0
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
            BaixaID = itemExtract.BaixaID,
            Unidade = itemExtract.Unidade,
            Nome = itemExtract.Nome,
            QtdeAEmpenhar = itemExtract.QtdeAEmpenhar,
            QtdeEmpenhada = itemExtract.QtdeEmpenhada,
            QtdeLicitada = itemExtract.QtdeLicitada,
            Saldo = itemExtract.Saldo,
            ValorEmpenhado = itemExtract.ValorEmpenhado,
            ValorLicitado = itemExtract.ValorLicitado,
            ValorUnitario = itemExtract.ValorUnitario
        };

        return itemDeBaixa;
    }

    public ItemDeBaixaDTO ObterParaExtracao(string nome, List<ItemDeBaixaDTO> itensBaixa)
    {
        nome = nome.Trim();

        var busca = itensBaixa.Where(w => w.Nome.ToUpper() == nome.ToUpper()).FirstOrDefault();

        if (busca is not null) return busca;

        if (nome.Contains("/"))
            return ObterComTratamentoDeNome(nome, '/', itensBaixa);

        if (nome.Contains(" "))
            return ObterComTratamentoDeNome(nome, ' ', itensBaixa);


        return new ItemDeBaixaDTO()
        {
            ID = 0,
            Unidade = " ",
            BaixaID = itensBaixa[0].BaixaID,
            Nome = $@"NÃO ENCONTRADO ({nome})",
            QtdeAEmpenhar = 0,
            QtdeEmpenhada = 0,
            QtdeLicitada = 0,
            Saldo = 0,
            ValorEmpenhado = 0,
            ValorLicitado = 0,
            ValorUnitario = 0
        };
    }
    private ItemDeBaixaDTO ObterComTratamentoDeNome(string nome, char separador, List<ItemDeBaixaDTO> itensBaixa)
    {
        var nomeComposto = nome.ToUpper().Split(separador).ToList();
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
                    .Where(w => w.Nome.ToUpper()
                    .Contains(
                        string.Concat(nomesDistinct[0], ' ', nomesDistinct[i])
                    )).ToList();

                if (item.Count == 1)
                    return item.First();
            }
        }

        var busca = itensBaixa
                    .Where(w => w.Nome.ToUpper()
                    .Contains(nomesDistinct[0]))
                    .ToList();

        if (busca.Count == 1)
            return busca.First();

        return new ItemDeBaixaDTO()
        {
            ID = 0,
            Unidade = " ",
            BaixaID = itensBaixa[0].BaixaID,
            Nome = $@"NÃO ENCONTRADO ({nome})",
            QtdeAEmpenhar = 0,
            QtdeEmpenhada = 0,
            QtdeLicitada = 0,
            Saldo = 0,
            ValorEmpenhado = 0,
            ValorLicitado = 0,
            ValorUnitario = 0
        };
    }
    private async Task<EntidadeDTO?> RetornaEntidade(EntidadeDeRetorno retorno, int tipo = 1)
    {
        var entidade = new EntidadeDTO();
        entidade.CNPJ = retorno.CNPJ ?? "";
        entidade.Telefone = "";
        entidade.Email = "";
        entidade.Nome = retorno.Nome ?? "";
        entidade.Fantasia = retorno.Nome ?? "";
        entidade.Endereco = new EnderecoDTO();
        entidade.Endereco.Logradouro ="";
        entidade.Endereco.Numero = "";
        entidade.Endereco.Cidade = "";
        entidade.Endereco.UF = "";
        entidade.Tipo = retorno.Tipo?? 1;
        entidade.ID = 0;
        entidade.IE = "";
        entidade.Status = 1;

        if (string.IsNullOrWhiteSpace(entidade.CNPJ)) return null;

        entidade = await _entidadeService.BuscaEntidadesPorCNPJ(entidade);

        return entidade;
    }
}

