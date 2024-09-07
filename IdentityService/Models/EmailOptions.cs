using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class EmailOptions
    {
       [Required]    
       public string Email { get; set; }=string.Empty;
       [Required]          
        public string DisplayName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; }= string.Empty;
        [Required]
        public string Host { get; set; } = string.Empty;
        [Required]
        public int Port { get; set; }
        
    }
}
