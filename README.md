# 🍔 Burger Queen

Este é um projeto para a matéria de POO de análise e desenvolvimento de sistemas da FATEC, do Prof. Me. Douglas Samuel Gonçalves.

---

O **BurguerQueen** é uma aplicação web que substitui o controle de pedidos feito no papel ou de cabeça. Com ele, o atendente cadastra produtos e clientes, abre pedidos com vários itens, acompanha o andamento de cada um (Aberto, Em preparo, Pronto, Entregue ou Cancelado) e registra o pagamento, com cálculo automático de total e de troco.

---

## 🎯 Visão geral

O sistema permite que o atendente de uma lanchonete:

- Cadastre, edite, consulte e exclua **produtos** (lanches, bebidas, porções e sobremesas);
- Cadastre e gerencie **clientes** (ou atenda no balcão);
- Abra **pedidos**, adicione e remova **itens** e acompanhe o **status** (Aberto → Em preparo → Pronto → Entregue, ou Cancelado);
- Registre o **pagamento** do pedido (dinheiro, Pix, débito ou crédito), com cálculo de troco quando for em dinheiro.

Todas as telas exibem **mensagens amigáveis** de sucesso e de erro, e as regras de negócio são validadas dentro das próprias entidades de domínio.

---

## 🛠 Tecnologias

| Framework | ASP.NET Core MVC (.NET 10) |
| Linguagem | C# |
| Acesso a dados | Dapper|

---

## 📁 Estrutura do projeto

```
BurguerQueen/
├── Controllers/                 # Apenas orquestram o fluxo (sem regra de negócio e sem SQL)
│   ├── HomeController.cs
│   ├── ProdutosController.cs
│   ├── ClientesController.cs
│   ├── PedidosController.cs
│   └── PagamentosController.cs
│
├── Models/                      # Entidades de domínio
│   ├── Produto.cs
│   ├── Lanche.cs
│   ├── Bebida.cs
│   ├── Cliente.cs
│   ├── Pedido.cs
│   ├── ItemPedido.cs
│   ├── Pagamento.cs
│   ├── Enums/                   # StatusPedido, FormaPagamento, TipoProduto
│   └── ViewModels/              # Modelos das telas, com Data Annotations
│
├── Repositories/
│   ├── Interfaces/              # IProdutoRepository, IClienteRepository, ...
│   └── Dapper/                  # Implementações com SQL parametrizado
│
├── Exceptions/
│   └── DominioException.cs      # Exceção para violações de regra de negócio
│
├── Views/
│   ├── Shared/                  # _Layout.cshtml, _ValidationScriptsPartial.cshtml
│   ├── Produtos/                # Index, Create, Edit, Details, Delete
│   ├── Clientes/
│   ├── Pedidos/
│   └── Pagamentos/
│
├── Database/
│   └── script.sql               # DDL (tabelas) + DML (dados de teste)
│
├── appsettings.json             # Connection string
└── Program.cs                   # Injeção de dependência e pipeline HTTP
```

## ▶️ Como rodar

### Pré-requisitos

* [.NET SDK 8.0 ou superior](https://dotnet.microsoft.com/download) instalado.
* Visual Studio Code, Visual Studio 2022 ou Rider.

### Passo a passo no terminal

1. Abra o terminal na pasta do projeto:

```
cd MeuProjetoMVC
```

2. Restaure as dependências NuGet:

```
dotnet restore
```

3. Execute a aplicação:

```
dotnet run
```

4. Acesse no navegador:
   * Acesse: `http://localhost:5000` ou a porta indicada no terminal (ex: `https://localhost:7001`).
   * Se o navegador reclamar do certificado HTTPS, rode `dotnet dev-certs https --trust`.

No **Visual Studio**, basta abrir o `.sln` e pressionar `F5`.

---

## 📄 Licença

Projeto desenvolvido para fins acadêmicos.
