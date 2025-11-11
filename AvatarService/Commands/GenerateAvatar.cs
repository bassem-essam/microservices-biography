using MediatR;

public class GenerateAvatar : IRequest<GenerateAvatarResult>
{ 
    public string Username { get; set; }
}

public class GenerateAvatarResult
{
    public string AvatarPath { get; set; }
}