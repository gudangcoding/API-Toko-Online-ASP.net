using System.ComponentModel.DataAnnotations;

namespace MyApiApp.Models
{
    public enum ActivityType
    {
        Login,
        Logout,
        OrderCreated,
        OrderUpdated,
        OrderCancelled,
        ProfileUpdated,
        PasswordChanged
    }

    public class Riwayat
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        public ActivityType ActivityType { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Details { get; set; } = string.Empty;
        
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}
