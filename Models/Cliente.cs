using BurguerQueen.Exceptions;

namespace BurguerQueen.Models
{
    public class Cliente
    {
        public string Id { get; private set; } = string.Empty;
        public string Nome { get; private set; } = string.Empty;
        public string Telefone { get; private set; } = string.Empty;
        public string CPF { get; private set; } = string.Empty;

        protected Cliente() { } // Construtor vazio (exigido pelo DAPPER)

        public Cliente(string nome, string telefone, string cpf) //garantindo validade 
        {
            Id = Guid.NewGuid().ToString();
            AtualizarDados(nome, telefone, cpf);
        }

        //Criar e editar encapsulamento de regras
        public void AtualizarDados(string nome, string telefone, string cpf)
        {
            if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3)
                throw new DominioException("O nome do cliente deve ter no mínimo 3 caracteres.");

            Nome = nome.Trim(); //tirar espaços em branco do início e do fim
            Telefone = LimparNumeros(telefone);
            CPF = LimparNumeros(cpf);

            if (!ValidarTelefone())
                throw new DominioException("Telefone inválido. Informe DDD + número (10 ou 11 dígitos).");

            if (!ValidarCPF())
                throw new DominioException("CPF inválido. Verifique os dígitos informados.");
        }

        //validar telefone fixo e celular
        public bool ValidarTelefone()
        {
            string numeros = LimparNumeros(Telefone);
            // Aceita telefone fixo (10 dígitos) ou celular com 9º dígito (11 dígitos)
            return numeros.Length == 10 || numeros.Length == 11;
        }

        //validar cpf com 11 dígitos e dígitos verificadores
        public bool ValidarCPF()
        {
            string cpf = LimparNumeros(CPF);

            if (cpf.Length != 11)
                return false;

            // Rejeita sequências repetidas como 11111111111, 00000000000, etc.
            if (cpf.Distinct().Count() == 1)
                return false;

            //seguindo tabela ASCII, na qual o '0' tem valor 48, '1' tem valor 49, e assim por diante. Subtraindo '0' de cada caractere, obtemos o valor numérico correspondente, sem precisar de int.Parse
            //Eu não sabia mas os digitos 10 e 11 do CPF são os dígitos verificadores, que são calculados a partir dos 9 primeiros dígitos. O primeiro dígito verificador é calculado usando os 9 primeiros dígitos, e o segundo dígito verificador é calculado usando os 9 primeiros dígitos mais o primeiro dígito verificador.

            // Cálculo do 1º dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if ((cpf[9] - '0') != digito1)
                return false;

            // Cálculo do 2º dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return (cpf[10] - '0') == digito2;
        }

        private static string LimparNumeros(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return string.Empty;
            return new string(valor.Where(char.IsDigit).ToArray());
        }
    }
}