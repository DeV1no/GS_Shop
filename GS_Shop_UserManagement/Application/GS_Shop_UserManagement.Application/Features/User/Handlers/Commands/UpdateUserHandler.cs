using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Commands;

public class UpdateUserHandler(IUserRepository repository, IMapper mapper, 
    IUploadStorageService<Domain.Entities.User> uploadStorageService) : IRequestHandler<UpdateUserCommand, int>
{
    public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.Get(request.UpdateUserDto.Id) ?? throw new Exception("User Not Found");
        mapper.Map(request.UpdateUserDto, user);

        if (request.UpdateUserDto.ProfilePic != null)
        {
           // var profilePictureDetail = await fileService.PostFileAsync(request.UpdateUserDto.ProfilePic, user.ProfilePictureId);
            var profilePictureDetail = await uploadStorageService.UploadFileAsync(request.UpdateUserDto.ProfilePic,request.UpdateUserDto.PreviousFilePath, cancellationToken);
            user.ProfilePicturePath = profilePictureDetail;
        }
        await repository.Update(user);
        return user.Id;
    }
}