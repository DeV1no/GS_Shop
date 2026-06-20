namespace EventBus.Messages.Events;

public class UserListEvent: IntegrationBaseEvent
{
    public int UserId { get; set; }
}