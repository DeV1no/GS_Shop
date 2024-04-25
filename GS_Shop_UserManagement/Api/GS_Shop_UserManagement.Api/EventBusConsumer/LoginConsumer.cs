using AutoMapper;
using EventBus.Messages.Events;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MassTransit;
using MediatR;
using UserClaimDto = EventBus.Messages.Events.UserClaimDto;
using UserClaimLimitationDto = EventBus.Messages.Events.UserClaimLimitationDto;

namespace GS_Shop_UserManagement.Api.EventBusConsumer;
public class LoginConsumer : IConsumer<LoginEvent>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public LoginConsumer(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
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
        var claims = response.Claim.Select(item => 
            new UserClaimDto {ClaimType = item.ClaimType}).ToList();
        var claimLimitation = response.ClaimLimitation.Select(item => 
            new UserClaimLimitationDto()
            {
                LimitationField = item.LimitationField,
                LimitedIds = item.LimitedIds,
                ClaimLimitationValue = item.ClaimLimitationValue
            }).ToList();
        // Send the response back to the publisher service
        await context.RespondAsync<LoginResponse>(new LoginResponse
        {
            Email = response.Email,
            ExpiresAt = response.ExpiresAt,
            Id = response.Id,
            Token = response.Token,
            UserName = response.UserName,
            ClaimLimitation = claimLimitation,
            Claim = claims,
            // Add other properties as needed
        });
    }
}




