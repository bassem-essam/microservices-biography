using MediatR;
using UserService.Data;
using UserService.Services;

public class DeleteUser : IRequest<DeleteUserResponse>
{
    public string Username { get; set; }
}

public class DeleteUserResponse
{
    public bool Deleted { get; set; }
    public string Error { get; set; }
}

public class DeleteUserHandler : IRequestHandler<DeleteUser, DeleteUserResponse>
{
    private readonly AppDbContext _context;
    private readonly IAvatarServiceClient _avatarServiceClient;
    public DeleteUserHandler(AppDbContext context, IAvatarServiceClient avatarServiceClient)
    {
        _context = context;
        _avatarServiceClient = avatarServiceClient;
    }

    public async Task<DeleteUserResponse> Handle(DeleteUser request, CancellationToken cancellationToken)
    {
        var user = _context.User.FirstOrDefault(x => x.Username == request.Username);

        var response = await _avatarServiceClient.DeleteAvatarAsync(user.Avatar);
        if (!response.Success)
        {
            throw new OperationFailed("Avatar deletion failed with error: " + response.Message);
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return new DeleteUserResponse { Deleted = true };
    }
}
