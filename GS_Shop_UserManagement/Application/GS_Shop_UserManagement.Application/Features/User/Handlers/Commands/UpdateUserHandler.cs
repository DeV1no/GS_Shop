using System.Globalization;
using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Commands;

public class UpdateUserHandler(IUserRepository repository, IMapper mapper, IFileService<Domain.Entities.User> fileService) : IRequestHandler<UpdateUserCommand, int>
{
    public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.Get(request.UpdateUserDto.Id) ?? throw new Exception("User Not Found");
        mapper.Map(request.UpdateUserDto, user);

        if (request.UpdateUserDto.ProfilePic != null)
        {
            var profilePictureDetail = await fileService.PostFileAsync(request.UpdateUserDto.ProfilePic, user.ProfilePictureId);
            user.ProfilePictureId = profilePictureDetail.Item1;
            user.ProfilePicturePath = profilePictureDetail.Item2;
        }

        await repository.Update(user);
        return user.Id;
    }
}