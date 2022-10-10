using MediatR;

namespace Chat.Domain.SeedWork;

public abstract class Entity
{
    private int? _requestedHashCode;
    private readonly List<INotification> _domainEvents = new();

    public virtual Guid Id { get; protected set; }
    public virtual Guid CreatedById { get; protected set; }
    public virtual DateTime DateCreated { get; protected set; }
    public virtual Guid? LastModifiedById { get; protected set; }
    public virtual DateTime? DateLastModified { get; protected set; }
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void SetId(Guid id) => Id = id;

    public void SetCreatedById(Guid id) => CreatedById = id;

    public void SetDateCreated(DateTime dt) => DateCreated = dt;

    public void SetLastModifiedById(Guid id) => LastModifiedById = id;

    public void SetDateLastModified(DateTime dt) => DateLastModified = dt;

    public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);

    public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool IsTransient() => Id == default;

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Entity)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        Entity item = (Entity)obj;
        if (item.IsTransient() || IsTransient())
            return false;

        return item.Id == Id;
    }

    public override int GetHashCode()
    {
        if (IsTransient())
            return base.GetHashCode();

        if (!_requestedHashCode.HasValue)
            _requestedHashCode = Id.GetHashCode() ^ 31;

        return _requestedHashCode.Value;
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (Equals(left, null))
            return Equals(right, null);
        else
            return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right) => !(left == right);
}
