using AutoMapper;
using EventBus.Messages.Events;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MassTransit;
using MediatR;
using UserClaimDto = EventBus.Messages.Events.UserClaimDto;
using UserClaimLimitationDto = EventBus.Messages.Events.UserClaimLimitationDto;

namespace GS_Shop_UserManagement.Api.EventBusConsumer;

public class RegisterConsumer(IMediator mediator, IMapper mapper)
    : IConsumer<RegisterEvent>
{
    public async Task Consume(ConsumeContext<RegisterEvent> context)
    {
        var registerDto = new RegisterUserDto
        {
            Password = context.Message.Password,
            UserName = context.Message.UserName,
            Email = context.Message.Email,
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
        };
        var command = new RegisterUserCommand {RegisterUserDto = registerDto};
        var response = await mediator.Send(command);
        // Send the response back to the publisher service
        await context.RespondAsync<RegisterResponse>(new RegisterResponse
        {
            UserId = response,
        });
    }
}