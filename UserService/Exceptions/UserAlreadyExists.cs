public class UserAlreadyExists : BusinessException
{
    public UserAlreadyExists(string username) : base($"User already exists: {username}")
    {
    }
}