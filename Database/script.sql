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