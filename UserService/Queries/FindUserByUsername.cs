using MediatR;

public class FindUserByUsername : IRequest<UserDTO>
{
    public string Username { get; set; }
    public bool ShouldNotifyVisit { get; set; }
}