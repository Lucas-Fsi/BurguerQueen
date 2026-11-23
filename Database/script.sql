PRAGMA foreign_keys = ON;

-- =====================
-- Tabela de produtos Lanches e Bebidas
-- Tipo 1 = Lanche/ Tipo 2 = Bebida
-- =====================
CREATE TABLE IF NOT EXISTS Produtos (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome TEXT NOT NULL,
    Preco REAL NOT NULL CHECK (Preco >= 0),
    TIPO INTEGER NOT NULL CHECK (TIPO IN (1, 2)),
    Descricao TEXT NULL,
    Adicional TEXT NULL,
    Tamanho REAL Null
)

-- Insert inicial com dados para teste:
INSERT INTO Produtos (Nome, Preco, Tipo, Descricao, Adicionais, Tamanho) VALUES
('Queen Classic', 28.90, 1, 'Pão brioche, hambúrguer 150g, queijo cheddar e molho da casa', 'Bacon, Cheddar extra', NULL),
('Double Queen', 36.50, 1, 'Pão australiano, dois hambúrgueres 150g, queijo prato e cebola caramelizada', 'Ovo, Cebola crispy', NULL),
('Chicken Crispy', 26.00, 1, 'Pão com gergelim, filé de frango empanado, alface americana e maionese temperada', 'Queijo cheddar, Picles', NULL),
('Veggie Queen', 29.90, 1, 'Pão brioche, hambúrguer de grão-de-bico, rúcula, tomate seco e muçarela', 'Molho pesto', NULL),
('Refrigerante Cola', 7.50, 2, NULL, NULL, 350),
('Refrigerante Guaraná', 7.00, 2, NULL, NULL, 350),
('Suco Natural de Laranja', 11.90, 2, NULL, NULL, 500),
('Milkshake de Chocolate', 18.50, 2, NULL, NULL, 400);


-- =====================
-- Tabela de pagamentos (uma tabela para todas as formas de pagamento)
-- Forma:      1 = Dinheiro / 2 = Pix / 3 = Cartao
-- Status:     0 = Pendente / 1 = Aprovado / 2 = Cancelado / 3 = Estornado
-- TipoCartao: 1 = Debito / 2 = Credito
-- Colunas especificas ficam NULL quando nao pertencem a forma do pagamento
-- =====================
CREATE TABLE IF NOT EXISTS Pagamentos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PedidoId INTEGER NOT NULL,
    Forma INTEGER NOT NULL CHECK (Forma IN (1, 2, 3)),
    Valor REAL NOT NULL CHECK (Valor > 0),
    DataHora TEXT NOT NULL,
    Status INTEGER NOT NULL CHECK (Status IN (0, 1, 2, 3)),
    ValorRecebido REAL NULL,
    Chave TEXT NULL,
    CodigoTransacao TEXT NULL,
    TipoCartao INTEGER NULL CHECK (TipoCartao IN (1, 2)),
    Parcelas INTEGER NULL,
    Bandeira TEXT NULL,
    CodigoAutorizacao TEXT NULL,
    FOREIGN KEY (PedidoId) REFERENCES Pedidos (Id)
);
