namespace Shared.Exceptions;

public class EntityNotFoundException : Exception
{
    private const string DefaultMessage = "Entity does not exist.";

    public EntityNotFoundException()
        : base(DefaultMessage)
    {
    }

    public EntityNotFoundException(object? entityId)
        : base($"Error: Entity with ID: '{entityId}' does not exist.")
    {
        EntityId = entityId;
    }

    public object? EntityId { get; }
}