using AutoMapper;
using EventBus.Messages.Events;
using GS_Shop_UserManagement.Application.Features.User.Requests.Queries;
using MassTransit;
using MediatR;

namespace GS_Shop_UserManagement.Api.EventBusConsumer;

public class GetUserPublicConsumer(IMediator mediator, IMapper mapper)
    : IConsumer<UserListPublicEvent>
{
    public async Task Consume(ConsumeContext<UserListPublicEvent> context)
    {
        var allocationList = await mediator.Send(new GetAllUserPublicRequest());
        await context.RespondAsync<UserListPublicResponseList>(new UserListPublicResponseList
        {
            Users = mapper.Map<List<UserListPublicResponse>>(allocationList)
        }); 
    }
}