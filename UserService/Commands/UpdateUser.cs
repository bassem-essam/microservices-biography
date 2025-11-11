using MediatR;
using UserService.Data;
using UserService.Services;

public class UpdateUser : IRequest<User>
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Biography { get; set; }
    public IFormFile Avatar { get; set; }
}

public class UpdateUserHandler : IRequestHandler<UpdateUser, User>
{
    private readonly AppDbContext _context;
    private readonly IAvatarServiceClient _avatarServiceClient;
    public UpdateUserHandler(AppDbContext context, IAvatarServiceClient avatarServiceClient)
    {
        _context = context;
        _avatarServiceClient = avatarServiceClient;
    }

    public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        var user = _context.User.FirstOrDefault(x => x.Username == request.Username);
        if (user == null) throw new UserNotFound(request.Username);

        if (request.Avatar != null)
        {
            var avatarResponse = await _avatarServiceClient.UploadAvatarAsync(request.Avatar);
            string newAvatarId = avatarResponse.AvatarPath;
            await _avatarServiceClient.DeleteAvatarAsync(user.Avatar);
            user.Avatar = newAvatarId;
        }

        if (request.Name != null)
                user.Name = request.Name;

        if (request.Biography != null)
            user.Biography = request.Biography;

        _context.User.Update(user);
        await _context.SaveChangesAsync();
        
        return user;
    }
}
