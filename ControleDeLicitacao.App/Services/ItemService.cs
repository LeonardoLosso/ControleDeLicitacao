using AutoMapper;
using ControleDeLicitacao.App.DTOs;
using ControleDeLicitacao.Domain.Entities.Cadastros;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.DTOs.Itens;
using ControleDeLicitacao.Common;
using ControleDeLicitacao.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.App.Services;

public class ItemService
{
    private readonly ItemRepository _itemRepository;
    private readonly IMapper _mapper;

    public ItemService(ItemRepository itemRepository, IMapper mapper)
    {
        _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    }

    public void Adicionar(ItemDTO dto)
    {

        var item = _mapper.Map<Item>(dto);

        if (item == null) return;

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
        _itemRepository.Adicionar(item);
    }


    public void Editar(ItemDTO dto, bool validarStatus = true)
    {
        if (validarStatus) ValidarInativo(dto.Status);

        var item = _mapper.Map<Item>(dto);

        if (item == null) return;

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

        _itemRepository.Editar(item);
    }

    //---------------------------[CONSULTAS]-------------------------------

    public ListagemDTO<ItemSimplificadoDTO> Listar(int? pagina, string? tipo, int? status, string? unidadePrimaria, string? unidadeSecundaria, string? search)
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


        var lista = query.Select(s =>
        new ItemSimplificadoDTO
        {
            ID = s.ID,
            Status = s.Status,
            EhCesta = s.EhCesta,
            Nome = s.Nome,
            UnidadePrimaria = s.UnidadePrimaria,
            UnidadeSecundaria = s.UnidadeSecundaria
        }).ToList();

        listagemDTO.Lista = lista;

        return listagemDTO;
    }

    public ItemDTO ObterPorID(int id)
    {
        var item = RetornarItem(id);

        if (item == null) return null;

        return _mapper.Map<ItemDTO>(item);
    }
    
    public ItemDTO ObterPorIDParaEdicao(int id)
    {
        var item = RetornarItem(id);

        if (item == null) return null;

        ValidarInativo(item.Status);

        return _mapper.Map<ItemDTO>(item);
    }
    private Item? RetornarItem(int id)
    {
        return _itemRepository.Buscar()
            .AsNoTracking()
            .Include(item => item.ListaItens)
                .ThenInclude(associativo => associativo.ItemFilho)
            .Include(item => item.ListaNomes)
            .SingleOrDefault(item => item.ID == id);

    }
    private void ValidarInativo(int status)
    {
        if (status == 2) throw new GenericException("Não é possivel editar um cadastro inativo", 501);
    }
}
