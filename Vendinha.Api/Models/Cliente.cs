namespace Vendinha.Api.Models;

public class Cliente
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public DateOnly DataNascimento { get; set; }
    public string? Email { get; set; }

    public List<Divida> Dividas { get; set; } = new();

    public int CalcularIdade()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var idade = hoje.Year - DataNascimento.Year;
        if (DataNascimento > hoje.AddYears(-idade)) idade--;
        return idade;
    }
}