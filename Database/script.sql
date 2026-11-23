PRAGMA foreign_keys = ON;

-- =====================
-- Produtos (Tipo 1 = Lanche / Tipo 2 = Bebida)
-- =====================
CREATE TABLE IF NOT EXISTS Produtos (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome       TEXT NOT NULL,
    Preco      REAL NOT NULL CHECK (Preco >= 0),
    Tipo       INTEGER NOT NULL CHECK (Tipo IN (1, 2)),
    Descricao  TEXT NULL,
    Adicionais TEXT NULL,
    Tamanho    REAL NULL
);

-- =====================
-- Pedidos
-- =====================
CREATE TABLE IF NOT EXISTS Pedidos (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId    INTEGER NULL,
    Status       INTEGER NOT NULL DEFAULT 0,
    DataCriacao  TEXT    NOT NULL
);

CREATE TABLE IF NOT EXISTS ItensPedido (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
    PedidoId       INTEGER NOT NULL,
    ProdutoId      INTEGER NOT NULL,
    NomeProduto    TEXT    NOT NULL,
    Quantidade     INTEGER NOT NULL CHECK (Quantidade > 0),
    PrecoUnitario  REAL    NOT NULL CHECK (PrecoUnitario > 0),
    FOREIGN KEY (PedidoId)  REFERENCES Pedidos(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
);

-- =====================
-- Dados de teste
-- =====================
INSERT INTO Produtos (Nome, Preco, Tipo, Descricao, Adicionais, Tamanho) VALUES
('Queen Classic', 28.90, 1, 'Pão brioche, hambúrguer 150g, queijo cheddar e molho da casa', 'Bacon, Cheddar extra', NULL),
('Double Queen', 36.50, 1, 'Pão australiano, dois hambúrgueres 150g, queijo prato e cebola caramelizada', 'Ovo, Cebola crispy', NULL),
('Chicken Crispy', 26.00, 1, 'Pão com gergelim, filé de frango empanado, alface americana e maionese temperada', 'Queijo cheddar, Picles', NULL),
('Veggie Queen', 29.90, 1, 'Pão brioche, hambúrguer de grão-de-bico, rúcula, tomate seco e muçarela', 'Molho pesto', NULL),
('Refrigerante Cola', 7.50, 2, NULL, NULL, 350),
('Refrigerante Guaraná', 7.00, 2, NULL, NULL, 350),
('Suco Natural de Laranja', 11.90, 2, NULL, NULL, 500),
('Milkshake de Chocolate', 18.50, 2, NULL, NULL, 400);