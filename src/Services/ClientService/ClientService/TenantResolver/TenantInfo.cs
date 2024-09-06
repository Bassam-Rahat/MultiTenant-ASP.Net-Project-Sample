using Finbuckle.MultiTenant.Abstractions;

namespace ClientService.API.TenantResolver
{
    public class TenantInfo : ITenantInfo
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}