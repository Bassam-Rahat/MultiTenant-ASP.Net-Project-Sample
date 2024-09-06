using Microsoft.AspNetCore.Identity;

namespace IdentityServer.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserTenant> UserTenants { get; set; }
    }
}