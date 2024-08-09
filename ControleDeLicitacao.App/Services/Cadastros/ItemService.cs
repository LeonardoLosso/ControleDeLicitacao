using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.DTOs.Itens;
using ControleDeLicitacao.Common;
using ControleDeLicitacao.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using ControleDeLicitacao.App.DTOs.Ata;
using System.ComponentModel;

namespace ControleDeLicitacao.App.Services.Cadastros;

public class ItemService
{
    private readonly ItemRepository _itemRepository;
    private readonly IMapper _mapper;

    public ItemService(ItemRepository itemRepository, IMapper mapper)
    {
        _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    }

    public async Task<ItemDTO> Adicionar(ItemDTO dto)
    {

        var item = _mapper.Map<Item>(dto);

        if (item is null) return null;

        if (item.EhCesta)
        {
            item.ListaItens = dto.ListaItens.Select(s =>
                new ItemAssociativo
                {
                    ItemFilhoID = s.ID
                }).ToList();
        }

        if (dto.ListaNomes.Any())
        {
            item.ListaNomes = dto.ListaNomes.Select(nome =>
                new ItemNomeAssociativo
                {
                    Nome = nome
                }).ToList();
        }
        await _itemRepository.Adicionar(item);
        return _mapper.Map<ItemDTO>(item);
    }


    public async Task Editar(ItemDTO dto, bool validarStatus = true)
    {
        if (validarStatus) ValidarInativo(dto.Status);

        var item = _mapper.Map<Item>(dto);

        if (item is null) return;

        if (item.EhCesta && dto.ListaItens != null)
        {
            item.ListaItens = dto.ListaItens.Select(s =>
                new ItemAssociativo
                {
                    ItemFilhoID = s.ID
                }).ToList();
        }

        if (dto.ListaNomes.Any())
        {
            item.ListaNomes = dto.ListaNomes.Select(nome =>
                new ItemNomeAssociativo
                {
                    Nome = nome
                }).ToList();
        }

        await _itemRepository.Editar(item);
    }

    //---------------------------[CONSULTAS]-------------------------------

    public async Task<ListagemDTO<ItemSimplificadoDTO>> Listar(int? pagina, string? tipo, int? status, string? unidadePrimaria, string? unidadeSecundaria, string? search)
    {
        var take = 15;
        var listagemDTO = new ListagemDTO<ItemSimplificadoDTO>();
        var query = _itemRepository.Buscar();

        //params
        if (!string.IsNullOrWhiteSpace(tipo))
        {
            if (tipo.Trim().Equals("Cesta"))
                query = query.Where(w => w.EhCesta == true);
            else
                query = query.Where(w => w.EhCesta == false);
        }

        if (status.HasValue)
            query = query.Where(w => w.Status == status);

        if (!string.IsNullOrWhiteSpace(unidadePrimaria))
            query = query.Where(w => w.UnidadePrimaria.Contains(unidadePrimaria.Trim()));

        if (!string.IsNullOrWhiteSpace(unidadeSecundaria))
            query = query.Where(w => w.UnidadeSecundaria.Contains(unidadeSecundaria.Trim()));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.BuscarPalavraChave(search);
        }

        query = query.OrderByDescending(o => o.Status == 1);

        listagemDTO.TotalItems = query.Count();

        if (pagina.HasValue)
        {
            listagemDTO.Page = pagina ?? 0;
            listagemDTO.CalcularTotalPage();

            var skip = (pagina.Value - 1) * take;
            query = query.Skip(skip).Take(take);
        }


        var lista = await query.Select(s =>
        new ItemSimplificadoDTO
        {
            ID = s.ID,
            Status = s.Status,
            EhCesta = s.EhCesta,
            Nome = s.Nome,
            UnidadePrimaria = s.UnidadePrimaria,
            UnidadeSecundaria = s.UnidadeSecundaria
        }).ToListAsync();

        listagemDTO.Lista = lista;

        return listagemDTO;
    }

    public async Task<ItemDTO?> ObterPorID(int id)
    {
        var item = await RetornarItem(id);

        if (item is null) return null;

        return _mapper.Map<ItemDTO>(item);
    }

    public async Task<ItemDTO?> ObterPorIDParaEdicao(int id)
    {
        var item = await RetornarItem(id);

        if (item is null) return null;

        ValidarInativo(item.Status);

        return _mapper.Map<ItemDTO>(item);
    }
    //------------------------------------------------------------------------------
    public async Task<ItemDTO> ObterParaExtracao(string nome)
    {
        nome = nome.Trim();

        var busca = await ObterPorNome(nome);

        if (busca is not null) return _mapper.Map<ItemDTO>(busca);

        if (nome.Contains("/"))
            return await ObterComTratamentoDeNome(nome, '/');

        if (nome.Contains(" "))
            return await ObterComTratamentoDeNome(nome, ' ');


        return new ItemDTO()
        {
            Id = 0,
            EhCesta = false,
            ListaItens = null,
            ListaNomes = new List<string>(),
            Status = 1,
            UnidadePrimaria = " ",
            UnidadeSecundaria = " ",
            Nome = $@"NÃO ENCONTRADO:{nome}"
        };
    }

    private async Task<Item?> ObterPorNome(string nome)
    {
        var busca = await _itemRepository
            .Buscar()
            .AsNoTracking()
            .Where(i => i.Nome == nome)
            .FirstOrDefaultAsync();

        return busca;
    }

    private async Task<List<Item>> ListarPorNome(string nome)
    {
        var busca = await _itemRepository
            .Buscar()
            .AsNoTracking()
            .Where(i => i.Nome.Contains(nome))
            .ToListAsync();

        return busca;
    }

    private async Task<ItemDTO> ObterComTratamentoDeNome(string nome, char separador = ' ')
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
                var item = await ListarPorNome(string.Concat(nomesDistinct[0], ' ', nomesDistinct[i]));
                if (item.Count == 1)
                    return _mapper.Map<ItemDTO>(item.First());
            }
        }

        var busca = await ListarPorNome(nomesDistinct[0]);
        if (busca.Count == 1)
            return _mapper.Map<ItemDTO>(busca.First());

        return new ItemDTO()
        {
            Id = 0,
            EhCesta = false,
            ListaItens = null,
            ListaNomes = new List<string>(),
            Status = 1,
            UnidadePrimaria = " ",
            UnidadeSecundaria = " ",
            Nome = $@"NÃO ENCONTRADO:{nome}"
        };
    }
    public async Task<List<ItemDeAtaDTO>> PreencherExtracao(List<ItemDeAtaDTO> itensAta)
    {
        var idAux = 100000;
        foreach (var item in itensAta)
        {
            var itemExtract = await ObterParaExtracao(item.Nome);
            if (itemExtract.Id == 0) itemExtract.Id = idAux++;

            item.ID = itemExtract.Id;
            item.Unidade = 
                !item.Unidade.Contains(" ") ? 
                    item.Unidade : item.Unidade.Split(' ')[0];
            item.Nome = itemExtract.Nome;
        }

        return itensAta;
    }

    //------------------------------------------------------------------------------

    private async Task<Item?> RetornarItem(int id)
    {
        return await _itemRepository.Buscar()
            .AsNoTracking()
            .Include(item => item.ListaItens)
                .ThenInclude(associativo => associativo.ItemFilho)
            .Include(item => item.ListaNomes)
            .SingleOrDefaultAsync(item => item.ID == id);

    }
    private void ValidarInativo(int status)
    {
        if (status == 2) throw new GenericException("Não é possivel editar um cadastro inativo", 501);
    }

}
