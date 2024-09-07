using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class JwtOptions
    {
        [Required]
        public string Issuer { get; set; } = string.Empty;
        [Required]
        public string Audience { get; set; }=string.Empty;
        [Required]
        public string SigningKey { get; set; }=string.Empty;
        [Required]
        public int Duration { get; set; }
    }
}
