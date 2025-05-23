﻿using ControleDeLicitacao.App.DTOs.Baixa;
using ControleDeLicitacao.App.Services.Documentos.Baixa;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeLicitacao.API.Controllers.Documentos.Baixa;

[Route("[controller]")]
public class EmpenhoController : BaseController
{
    private readonly EmpenhoService _service;
    public EmpenhoController(EmpenhoService service) : base()
    {
        _service = service;
    }

    [HttpGet("obter/{id}")]
    public async Task<IActionResult> ObterPorID(int id)
    {
        await base.ValidaRecurso(407);

        var dto = await _service.ObterPorID(id);

        if (dto is null) return NotFound();

        return Ok(dto);
    }

    [HttpGet("existe/{id}")]
    public async Task<IActionResult> VerificaExistencia(int id)
    {
        var possui = await _service.PossuiEmpenho(id);

        return Ok(possui);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Listar(int id)
    {
        var lista = await _service.Listar(id);
        return Ok(lista);
    }

    [HttpGet("itens/{id}")]
    public async Task<IActionResult> ListarItens(int id, [FromQuery] string? search)
    {
        var lista = await _service.ListarItens(id, search);
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Novo([FromBody] BaixaDTO dto)
    {
        await base.ValidaRecurso(402);

        var novo = await _service.Adicionar(dto);

        return await RetornaNovo(novo);
    }

    [HttpDelete]
    public async Task<IActionResult> Excluir([FromQuery] int id)
    {
        await base.ValidaRecurso(403);

        await _service.Excluir(id);
        return await RetornaDelete(id);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] JsonPatchDocument<EmpenhoDTO> patchDoc)
    {
        await base.ValidaRecurso(406);

        var dto = await _service.ObterPorID(id);
        if(dto is null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(dto, ModelState);
        }
        catch (JsonPatchException e)
        {
            return BadRequest(e.Message);
        }

        await _service.Editar(dto);

        return await RetornaEdicao(patchDoc);
    }

    [HttpPatch("status/{id}")]
    public async Task<IActionResult> AlteraStatus(int id, JsonPatchDocument<EmpenhoDTO> patchDoc)
    {
        await base.ValidaRecurso(405);
        var dto = await _service.ObterPorID(id);
        if (dto is null)
        {
            return NotFound();
        }

        try
        {
            patchDoc.ApplyTo(dto, ModelState);
        }
        catch (JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }

        await _service.Editar(dto, true);

        return await RetornaEdicao(patchDoc);
    }
}
