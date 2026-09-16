namespace VaultCore.SharedKernel.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class InsufficientStockException : DomainException
{
    public Guid MiniatureId { get; }
    public int Requested { get; }
    public int Available { get; }

    public InsufficientStockException(Guid miniatureId, int requested, int available)
        : base($"Insufficient stock for miniature {miniatureId}. Requested: {requested}, Available: {available}")
    {
        MiniatureId = miniatureId;
        Requested = requested;
        Available = available;
    }
}
