namespace EventBus.Messages.Events;

public class IntegrationBaseEvent(Guid id, DateTime createDate)
{
    public IntegrationBaseEvent() : this(Guid.NewGuid(), DateTime.Now)
    {
    }

    public Guid Id { get; private set; } = id;
    private DateTime CreateDate { get; set; } = createDate;
}