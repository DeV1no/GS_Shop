using AutoMapper;
using EventBus.Messages.Events;
using GS_Shop_UserManagement.Application.Features.User.Requests.Queries;
using MassTransit;
using IMediator = MediatR.IMediator;

namespace GS_Shop_UserManagement.Api.EventBusConsumer;

public class GetUserConsumer : IConsumer<UserListEvent>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GetUserConsumer(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<UserListEvent> context)
    {
        var allocationList = await _mediator.Send(new GetAllUserRequest());
        await context.RespondAsync<List<UserListResponse>>(_mapper.Map<List<UserListResponse>>(allocationList)); 
    }
}