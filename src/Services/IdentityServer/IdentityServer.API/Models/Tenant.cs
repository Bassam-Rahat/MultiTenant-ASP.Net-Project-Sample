namespace IdentityServer.API.Models
{
    public class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserTenant> UserTenants { get; set; }
    }
}