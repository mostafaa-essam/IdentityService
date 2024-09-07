using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models
{
    public class User:IdentityUser
    {
        public string FullName { get; set; }=string.Empty;
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}
