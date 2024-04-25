namespace EventBus.Messages.Events;

public class UserClaimLimitationDto
{
    public string ClaimLimitationValue { get; set; } = string.Empty;
    public string LimitedIds { get; set; } = string.Empty;
    public string LimitationField { get; set; } = string.Empty;
}