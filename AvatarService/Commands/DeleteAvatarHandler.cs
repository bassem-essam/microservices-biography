using AvatarService.Services;
using MediatR;

public class DeleteAvatarHandler : IRequestHandler<DeleteAvatar, DeleteAvatarResult>
{
    private readonly IAvatarStore _avatarStore;
    public DeleteAvatarHandler(IAvatarStore avatarStore)
    {
        _avatarStore = avatarStore;
    }

    public Task<DeleteAvatarResult> Handle(DeleteAvatar request, CancellationToken cancellationToken)
    {
        if (!_avatarStore.Exists(request.AvatarPath))
        {
            return Task.FromResult(new DeleteAvatarResult { Success = false, Message = "Avatar not found" });
        }

        _avatarStore.Delete(request.AvatarPath);
        return Task.FromResult(new DeleteAvatarResult { Success = true });
    }
}