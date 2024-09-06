using IdentityServer.API;
using IdentityServer.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public TenantController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> AddTenant([FromBody] TenantRegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingTenant = await _dbContext.Tenants.FindAsync(model.Id);
        if (existingTenant != null)
        {
            return Conflict("A tenant with the same ID already exists.");
        }

        var tenant = new Tenant
        {
            Id = model.Id,
            Name = model.Name
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        return Ok("Tenant added successfully.");
    }
}