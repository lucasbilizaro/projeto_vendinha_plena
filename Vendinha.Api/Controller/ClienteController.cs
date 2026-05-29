using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Data;
using Vendinha.Api.Models;

namespace Vendinha.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{
    private readonly VendinhaDbContext _context;

    public ClienteController(VendinhaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? nome, [FromQuery] int pagina = 1)
    {
        if (pagina < 1) pagina = 1;
        const int tamanhoPagina = 10;

        var query = _context.Clientes.Include(c => c.Dividas).AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(c => c.NomeCompleto.ToLower().Contains(nome.ToLower()));
        }

        var listaClientes = await query.ToListAsync();

        var resultado = listaClientes.Select(c => new
        {
            c.Id,
            c.NomeCompleto,
            c.CPF,
            c.DataNascimento,
            c.Email,
            Idade = c.CalcularIdade(),
            TotalDividas = c.Dividas.Where(d => !d.Paga).Sum(d => d.Valor)
        })
        .OrderByDescending(c => c.TotalDividas)
        .Skip((pagina - 1) * tamanhoPagina)
        .Take(tamanhoPagina)
        .ToList();

        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Dividas)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null) return NotFound("Cliente não encontrado.");

        return Ok(new
        {
            cliente.Id,
            cliente.NomeCompleto,
            cliente.CPF,
            cliente.DataNascimento,
            cliente.Email,
            Idade = cliente.CalcularIdade(),
            Dividas = cliente.Dividas
        });
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.NomeCompleto))
            return BadRequest("Nome completo é obrigatório.");

        if (string.IsNullOrWhiteSpace(cliente.CPF) || cliente.CPF.Length != 11)
            return BadRequest("CPF inválido. Deve conter 11 dígitos.");

        if (cliente.DataNascimento == default)
            return BadRequest("Data de nascimento inválida.");

        if (!string.IsNullOrWhiteSpace(cliente.Email) && !cliente.Email.Contains("@"))
            return BadRequest("O e-mail informado é inválido.");

        var cpfExiste = await _context.Clientes.AnyAsync(c => c.CPF == cliente.CPF);
        if (cpfExiste) return BadRequest("Já existe um cliente cadastrado com este CPF.");

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Cliente clienteAtualizado)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound("Cliente não encontrado.");

        if (string.IsNullOrWhiteSpace(clienteAtualizado.NomeCompleto))
            return BadRequest("Nome completo é obrigatório.");

        if (cliente.CPF != clienteAtualizado.CPF)
        {
            var cpfExiste = await _context.Clientes.AnyAsync(c => c.CPF == clienteAtualizado.CPF);
            if (cpfExiste) return BadRequest("O novo CPF informado já está em uso.");
            cliente.CPF = clienteAtualizado.CPF;
        }

        if (!string.IsNullOrWhiteSpace(clienteAtualizado.Email) && !clienteAtualizado.Email.Contains("@"))
            return BadRequest("O e-mail informado é inválido.");

        cliente.NomeCompleto = clienteAtualizado.NomeCompleto;
        cliente.DataNascimento = clienteAtualizado.DataNascimento;
        cliente.Email = clienteAtualizado.Email;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound("Cliente não encontrado.");

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}