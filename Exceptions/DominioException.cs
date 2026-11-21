namespace BurguerQueen.Exceptions;
// exception personalizada de regra de negócio
// herdara de System.Exception para ser lançada com throw e reconhecida com o Try-Catch
public class DominioException : Exception
{
    // Contrutor que recebe a mensagem mais amigaqvel de erro e repassa para a classe base Exception
    public DominioException(string message) : base(message)
    {
    }
}