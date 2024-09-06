namespace IdentityServer.API.Models
{
    public class RegisterUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string TenantId { get; set; } 
    }
}