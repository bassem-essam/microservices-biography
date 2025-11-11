using MediatR;

public class DeleteAvatar : IRequest<DeleteAvatarResult>
{ 
    public string AvatarPath { get; set; }
}

public class DeleteAvatarResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}