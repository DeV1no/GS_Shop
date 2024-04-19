namespace EventBus.Messages.Events;

public class LoginEvent : IntegrationBaseEvent
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}