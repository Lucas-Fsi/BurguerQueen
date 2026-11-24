# 🍔 Burger Queen

Este é um projeto para a matéria de POO de análise e desenvolvimento de sistemas da FATEC, do Prof. Me. Douglas Samuel Gonçalves.

---

O **BurguerQueen** é uma aplicação web que substitui o controle de pedidos feito no papel ou de cabeça. Com ele, o atendente cadastra produtos e clientes, abre pedidos com vários itens, acompanha o andamento de cada um (Aberto, Em preparo, Pronto, Entregue ou Cancelado) e registra o pagamento, com cálculo automático de total e de troco.

## Equipe

- Lucas Cunha Barbosa - RA: 2840482511027
- Víctor Gabriel Souza Campesi - RA: 2840482511033
- Leonardo Sousa Gomes de Araujo - RA: 2840482511018
- Jéssica Bianchi Vieira da Silva - RA: 2840482511014


## 🎯 Visão geral

O sistema permite que o atendente de uma lanchonete:

- Cadastre, edite, consulte e exclua **produtos** (lanches, bebidas, porções e sobremesas);
- Cadastre e gerencie **clientes** (ou atenda no balcão);
- Abra **pedidos**, adicione e remova **itens** e acompanhe o **status** (Aberto → Em preparo → Pronto → Entregue, ou Cancelado);
- Registre o **pagamento** do pedido (dinheiro, Pix, débito ou crédito), com cálculo de troco quando for em dinheiro.

Todas as telas exibem **mensagens amigáveis** de sucesso e de erro, e as regras de negócio são validadas dentro das próprias entidades de domínio.

## 🛠 Tecnologias

| Framework | ASP.NET Core MVC (.NET 10) |
| Linguagem | C# |
| Acesso a dados | Dapper|

## 📁 Estrutura do projeto

```
BurguerQueen/
├── Assets/
│   └── LogoBQ.jpg
│
├── Controllers/                 # Apenas orquestram o fluxo (sem regra de negócio e sem SQL)
│   ├── ClientesController.cs
│   ├── HomeController.cs
│   ├── PagamentosController.cs
│   ├── PedidosController.cs
│   └── ProdutosController.cs
│
├── Models/                      # Entidades de domínio
│   ├── Bebida.cs
│   ├── Cartao.cs
│   ├── Cliente.cs
│   ├── Dinheiro.cs
│   ├── ErrorViewModel.cs
│   ├── ItemPedido.cs
│   ├── Lanche.cs
│   ├── Pagamento.cs
│   ├── Pedido.cs
│   ├── Pix.cs
│   ├── Produto.cs
│   ├── Enums/                   # FormaPagamento, StatusPagamento, StatusPedido, TipoCartao, TipoProduto e extensões
│   └── ViewModels/              # Modelos das telas, com Data Annotations
│
├── Repositories/
│   ├── Interfaces/              # IRepository, IClienteRepository, IPagamentoRepository, IPedidoRepository, IProdutoRepository
│   └── Dapper/                  # Implementações com SQL parametrizado
│
├── Services/                    # Regras que coordenam mais de uma entidade (ex.: Pedido e Pagamento)
│   ├── PagamentoService.cs
│   └── PedidoService.cs
│
├── Exceptions/
│   └── DominioException.cs      # Exceção para violações de regra de negócio
│
├── Views/
│   ├── _ViewImports.cshtml
│   ├── _ViewStart.cshtml
│   ├── Clientes/                # Create, Details, Edit, Index
│   ├── Home/                    # Index, Privacy
│   ├── Pagamentos/              # Create, Details
│   ├── Pedidos/                 # Create, Details, Index
│   ├── Produtos/                # Create, Delete, Details, Edit, Index
│   └── Shared/                  # _Layout.cshtml, _ValidationScriptsPartial.cshtml, Error.cshtml
│
├── Database/
│   └── script.sql               # DDL (tabelas) + DML (dados de teste)
│
├── Properties/
│   └── launchSettings.json
│
├── appsettings.json             # Connection string
├── appsettings.Development.json
├── BurguerQueen.csproj
└── Program.cs                   # Injeção de dependência e pipeline HTTP
```

## ▶️ Como rodar

### Pré-requisitos

* [.NET SDK 10.0 ou superior](https://dotnet.microsoft.com/download) instalado.
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
