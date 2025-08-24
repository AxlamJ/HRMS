using System.ComponentModel.DataAnnotations;

namespace HrManagement.Models
{
    public class LoginAttempt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(45)]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public DateTime AttemptTime { get; set; }

        [Required]
        public bool IsSuccessful { get; set; }
    }
} 