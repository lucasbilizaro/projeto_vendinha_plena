# Vendinha Plena

Sistema de controle de dívidas de clientes de uma vendinha.

## Como rodar

1. Instalar .NET 10 SDK
2. Clonar o repositório
3. Entrar na pasta `Vendinha.Api`
4. Rodar `dotnet run`
5. Testar os endpoints via Postman ou navegador:
   - `http://localhost:5052/cliente`
   - `http://localhost:5052/divida/cliente/{id}`

## Endpoints

### Clientes
- GET /cliente — lista todos
- GET /cliente/{id} — busca por ID
- POST /cliente — cria cliente
- PUT /cliente/{id} — atualiza cliente
- DELETE /cliente/{id} — remove cliente e suas dívidas

### Dívidas
- GET /divida/cliente/{clienteId} — lista dívidas do cliente
- POST /divida — cria dívida (não é possível ter mais de uma em aberto)
- PUT /divida/{id}/pagar — marca como paga

## Banco de dados

SQLite — o arquivo `vendinha.db` é criado automaticamente ao rodar.
O script de criação das tabelas está em `schema.sql`.
