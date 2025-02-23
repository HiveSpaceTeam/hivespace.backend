using HiveSpace.Domain.Seedwork;
using MediatR;

namespace HiveSpace.Domain.SeedWork;
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    where TKey : struct, IEquatable<TKey>
{
    private List<INotification> _domainEvents = [];
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents = _domainEvents ?? [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}
