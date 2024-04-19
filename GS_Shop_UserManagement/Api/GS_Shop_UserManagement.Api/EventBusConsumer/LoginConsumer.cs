using EventBus.Messages.Events;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MassTransit;
using MediatR;

namespace GS_Shop_UserManagement.Api.EventBusConsumer;
public class LoginConsumer : IConsumer<LoginEvent>
{
    private readonly IMediator _mediator;

    public LoginConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<LoginEvent> context)
    {
        var loginDto = new LoginDto
        {
            Password = context.Message.Password,
            UserName = context.Message.UserName,
        };
        var command = new LoginUserCommand { LoginDto = loginDto };
        var response = await _mediator.Send(command);

        // Send the response back to the publisher service
        await context.RespondAsync<LoginResponse>(new LoginResponse
        {
            Email = response.Email,
            ExpiresAt = response.ExpiresAt,
            Id = response.Id,
            Token = response.Token,
            UserName = response.UserName,
            // Add other properties as needed
        });
    }
}




