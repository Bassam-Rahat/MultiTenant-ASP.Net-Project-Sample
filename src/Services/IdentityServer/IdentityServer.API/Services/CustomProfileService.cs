using IdentityServer.API.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer.API.Services
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public CustomProfileService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user == null) throw new ArgumentException("Invalid user!");

            var userTenants = await _dbContext.UserTenants
                .Where(ut => ut.UserId == user.Id)
                .Select(ut => ut.TenantId)
                .ToListAsync();

            var claims = new List<Claim>
        {
            new Claim("user_id", user.Id)
        };
            claims.AddRange(userTenants.Select(t => new Claim("tenant", t)));

            context.IssuedClaims.AddRange(claims);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}