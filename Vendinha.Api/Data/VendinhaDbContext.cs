using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Models;

namespace Vendinha.Api.Data;

public class VendinhaDbContext : DbContext
{
    public VendinhaDbContext(DbContextOptions<VendinhaDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Divida> Dividas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("Clientes");
            e.HasKey(c => c.Id);
            e.Property(c => c.NomeCompleto).HasColumnName("nome_completo").IsRequired().HasMaxLength(200);
            e.Property(c => c.CPF).HasColumnName("cpf").IsRequired().HasMaxLength(11);
            e.HasIndex(c => c.CPF).IsUnique();
            e.Property(c => c.DataNascimento).HasColumnName("data_nascimento");
            e.Property(c => c.Email).HasColumnName("email").HasMaxLength(200);
        });

        modelBuilder.Entity<Divida>(e =>
        {
            e.ToTable("Dividas");
            e.HasKey(d => d.Id);
            e.Property(d => d.Valor).HasColumnName("valor").HasColumnType("decimal(10,2)");
            e.Property(d => d.Paga).HasColumnName("paga");
            e.Property(d => d.DataCriacao).HasColumnName("data_criacao");
            e.Property(d => d.DataPagamento).HasColumnName("data_pagamento");
            e.Property(d => d.ClienteId).HasColumnName("cliente_id");
            e.HasOne(d => d.Cliente)
             .WithMany(c => c.Dividas)
             .HasForeignKey(d => d.ClienteId);
        });
    }
}