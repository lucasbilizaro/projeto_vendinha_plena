namespace Vendinha.Api.Models;

public class Divida
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public bool Paga { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataPagamento { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; } 
}