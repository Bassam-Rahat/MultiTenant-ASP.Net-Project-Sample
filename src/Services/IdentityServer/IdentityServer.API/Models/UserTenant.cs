namespace IdentityServer.API.Models
{
    public class UserTenant
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}