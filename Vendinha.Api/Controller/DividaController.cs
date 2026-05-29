using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Data;
using Vendinha.Api.Models;

namespace Vendinha.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DividaController : ControllerBase
{
    private readonly VendinhaDbContext _context;

    public DividaController(VendinhaDbContext context)
    {
        _context = context;
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetPorCliente(int clienteId)
    {
        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == clienteId);
        if (!clienteExiste) return NotFound("Cliente não encontrado.");

        var dividas = await _context.Dividas
            .Where(d => d.ClienteId == clienteId)
            .ToListAsync();

        return Ok(dividas);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Divida divida)
    {
        if (divida.Valor <= 0) return BadRequest("O valor da dívida deve ser maior que zero.");

        var cliente = await _context.Clientes
            .Include(c => c.Dividas)
            .FirstOrDefaultAsync(c => c.Id == divida.ClienteId);

        if (cliente == null) return NotFound("Cliente não cadastrado.");

        var possuiDividaAtiva = cliente.Dividas.Any(d => !d.Paga);
        if (possuiDividaAtiva)
            return BadRequest("Não é possível pendurar uma nova conta. O cliente já possui uma dívida em aberto.");

        divida.Paga = false;
        divida.DataCriacao = DateTime.Now;
        divida.DataPagamento = null;

        _context.Dividas.Add(divida);
        await _context.SaveChangesAsync();

        return Created($"/divida/cliente/{divida.ClienteId}", divida);
    }

    [HttpPut("{id}/pagar")]
    public async Task<IActionResult> Pagar(int id)
    {
        var divida = await _context.Dividas.FindAsync(id);
        if (divida == null) return NotFound("Dívida não encontrada.");

        if (divida.Paga) return BadRequest("Esta dívida já foi paga.");

        divida.Paga = true;
        divida.DataPagamento = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(new { Mensagem = "Dívida quitada com sucesso!", Divida = divida });
    }
}