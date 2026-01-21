namespace Compras.Application.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base("La solicitud contiene datos inválidos.")
    {
        Errors = errors.ToArray();
    }
}
