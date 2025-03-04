using MediatR;

namespace HiveSpace.Domain.Seedwork;

public interface IAggregateRoot 
{
    void AddDomainEvent(INotification eventItem);

    void RemoveDomainEvent(INotification eventItem);

    void ClearDomainEvents();
}
