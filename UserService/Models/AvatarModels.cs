using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class GenerateAvatarRequest
    {
        [Required]
        public string Username { get; set; }
    }

    public class DeleteAvatarRequest
    {
        [Required]
        public string AvatarPath { get; set; }
    }

    public class AvatarResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AvatarPath { get; set; }
        public string AvatarUrl { get; set; }
    }
}