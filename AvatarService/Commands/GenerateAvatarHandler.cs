using AvatarService.Services;
using MediatR;

public class GenerateAvatarHandler : IRequestHandler<GenerateAvatar, GenerateAvatarResult>
{
    private readonly AvatarGenerationService _avatarGenerationService;
    public GenerateAvatarHandler(AvatarGenerationService avatarGenerationService)
    {
        _avatarGenerationService = avatarGenerationService;
    }
    public Task<GenerateAvatarResult> Handle(GenerateAvatar request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Generating avatar for " + request.Username);
        var fileName = _avatarGenerationService.GenerateAvatar(request.Username);
        return Task.FromResult(new GenerateAvatarResult { AvatarPath = fileName });
    }
}
