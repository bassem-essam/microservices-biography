public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Biography { get; set; }
    public string Avatar { get; set; }
    public int VisitCount { get; set; }
    // public int TopUserRank { get; set; }

    public static UserDTO FromUser(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Name = user.Name,
            Biography = user.Biography,
            Avatar = user.Avatar
        };
    }
}
