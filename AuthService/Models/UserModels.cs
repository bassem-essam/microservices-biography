using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; }
    }

    public class UserServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}