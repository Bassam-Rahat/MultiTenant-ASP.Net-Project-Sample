using ClientService.Infrastructure;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using ProductService.Inftrastructure;
using TenantInfo = ClientService.API.TenantResolver.TenantInfo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMultiTenant<TenantInfo>()
    .WithInMemoryStore(options =>
    {
        options.Tenants.Add(new TenantInfo
        {
            Id = "tenant1",
            Identifier = "tenant1",
            Name = "Tenant 1",
            ConnectionString = "Server=localhost;Initial Catalog=Tenant1;Integrated Security=True"
        });

        options.Tenants.Add(new TenantInfo
        {
            Id = "tenant2",
            Identifier = "tenant2",
            Name = "Tenant 2",
            ConnectionString = "Server=localhost;Initial Catalog=Tenant2;Integrated Security=True"
        });
    })
    .WithHeaderStrategy("X-Tenant-Id");

builder.Services.AddDbContext<ClientDbContext>((serviceProvider, options) =>
{
    var tenantInfo = serviceProvider.GetRequiredService<IMultiTenantContextAccessor<TenantInfo>>().MultiTenantContext?.TenantInfo;

    if (tenantInfo != null)
    {
        options.UseSqlServer(tenantInfo.ConnectionString);
    }
});

builder.Services.AddDbContext<ProductDbContext>((serviceProvider, options) =>
{
    var tenantInfo = serviceProvider.GetRequiredService<IMultiTenantContextAccessor<TenantInfo>>().MultiTenantContext?.TenantInfo;

    if (tenantInfo != null)
    {
        options.UseSqlServer(tenantInfo.ConnectionString);
    }
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
ApplyMigrationsForAllTenants(app);
app.UseMultiTenant();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();



void ApplyMigrationsForAllTenants(IApplicationBuilder app)
{
    // Path to your SQL migration script file
    var sqlScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Migrations", "migrationScript.sql");

    if (!File.Exists(sqlScriptPath))
    {
        // File does not exist, do nothing
        return; // Exit the method without doing anything
    }

    // Read the entire SQL script into a string
    var migrationScript = File.ReadAllText(sqlScriptPath);

    // Split the script by "GO" since it's a batch separator
    var sqlBatches = migrationScript.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

    using (var scope = app.ApplicationServices.CreateScope())
    {
        var multiTenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<TenantInfo>>();
        var tenants = multiTenantStore.GetAllAsync().Result; // Get all tenants

        foreach (var tenant in tenants)
        {
            // Dynamically configure DbContext for tenant's connection string
            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseSqlServer(tenant.ConnectionString); // Use tenant-specific connection string

            // Instantiate DbContext with dynamically built options
            using (var dbContext = new ProductDbContext(optionsBuilder.Options))
            {
                foreach (var batch in sqlBatches)
                {
                    try
                    {
                        // Execute each batch separately
                        dbContext.Database.ExecuteSqlRaw(batch);
                    }
                    catch (Exception ex)
                    {
                        // Log the error and continue executing the remaining batches
                        Console.WriteLine($"Error executing migration script for tenant {tenant.Identifier}: {ex.Message}");
                    }
                }
            }
        }
    }
}