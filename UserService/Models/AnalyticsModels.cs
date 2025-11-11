using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class AnalyticsRequest
    {
        [Required]
        public string Username { get; set; }
    }

    public class TopUser {
        public string Username { get; set; }
        public int VisitCount { get; set; }
    }
}