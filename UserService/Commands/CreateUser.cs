using System.Text.Json.Nodes;
using Azure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using UserService.Data;
using UserService.Services;

public class CreateUser : IRequest<User>
{
    public string Username { get; set; }
}

public class CreateUserHandler : IRequestHandler<CreateUser, User>
{
    private readonly AppDbContext _context;
    private readonly IAvatarServiceClient _avatarServiceClient;
    private readonly EventPublisher _eventPublisher;
    public CreateUserHandler(
        AppDbContext context,
        IAvatarServiceClient avatarServiceClient,
        EventPublisher eventPublisher)
    {
        _context = context;
        _avatarServiceClient = avatarServiceClient;
        _eventPublisher = eventPublisher;
    }

    public async Task<User> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        if (_context.User.Any(x => x.Username == request.Username))
        {
            throw new UserAlreadyExists(request.Username);
        }

        string profilePic = "";
        try
        {
            var response = await _avatarServiceClient.GenerateAvatarAsync(request.Username);
            profilePic = response.AvatarPath;
       }
        catch (Exception ex)
        {
            throw new OperationFailed("User was not created because avatar generation failed", ex);
        }

        User user = new User
        {
            Username = request.Username,
            Name = request.Username,
            Biography = "Default Biography",
            Avatar = profilePic
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        _eventPublisher.PublishUserCreated(request.Username);

        return user;
    }
}
