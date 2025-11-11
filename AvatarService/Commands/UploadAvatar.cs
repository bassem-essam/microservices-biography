using MediatR;

public class UploadAvatar : IRequest<UploadAvatarResult>
{
    public IFormFile Avatar { get; set; }
}

public class UploadAvatarResult
{
    public bool Created { get; set; }
    public string? AvatarPath { get; set; }
    public string? Error { get; set; }
}