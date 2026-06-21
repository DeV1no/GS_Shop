using System.Collections.Generic;

namespace EventBus.Messages.Events;

public class UserListPublicResponseList
{
    public List<UserListPublicResponse> Users { get; set; } = new();
}
