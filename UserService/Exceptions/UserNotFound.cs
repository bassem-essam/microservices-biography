public class UserNotFound : BusinessException
{
    public UserNotFound(string username) : base($"User not found: {username}")
    {
    }
}