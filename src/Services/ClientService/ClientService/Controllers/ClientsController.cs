using ClientService.API.DTOs;
using ClientService.Infrastructure;
using ClientService.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Inftrastructure;

namespace ClientService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientDbContext _clientDbContext;
        private readonly ProductDbContext _productDbContext;

        public ClientsController(ClientDbContext clientDbContext, ProductDbContext productDbContext)
        {
            _clientDbContext = clientDbContext;
            _productDbContext = productDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            // Step 1: Get all clients
            var clients = await _clientDbContext.Clients.ToListAsync();

            // Step 2: Get all ClientProduct mappings
            var clientProducts = await _clientDbContext.ClientProducts.ToListAsync();

            // Step 3: Get the product IDs from the client-product mappings
            var productIds = clientProducts.Select(cp => cp.ProductId).Distinct().ToList();

            // Step 4: Fetch the associated products from ProductDbContext
            var products = await _productDbContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Step 5: Use LINQ to map Clients and their associated Products into the DTO
            var clientDtos = clients.Select(client => new ClientDto
            {
                ClientId = client.Id,
                ClientName = client.Name,
                Products = clientProducts
                    .Where(cp => cp.ClientId == client.Id)
                    .Select(cp => {
                        var product = products.FirstOrDefault(p => p.Id == cp.ProductId);
                        return new ProductDto
                        {
                            ProductId = product.Id,
                            ProductName = product.Name
                        };
                    }).ToList()
            }).ToList();

            // Step 6: Return the DTO
            return Ok(clientDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] Client client)
        {
            // Add the new client
            _clientDbContext.Clients.Add(client);
            await _clientDbContext.SaveChangesAsync();

            return Ok(client);
        }

        [HttpPost("{clientId}/products/{productId}")]
        public async Task<IActionResult> AddProductToClient(int clientId, int productId)
        {
            // Fetch the client by clientId from the ClientDbContext
            var client = await _clientDbContext.Clients.FindAsync(clientId);
            if (client == null) return NotFound("Client not found");

            // Fetch the product by productId from the ProductDbContext
            var product = await _productDbContext.Products.FindAsync(productId);
            if (product == null) return NotFound("Product not found");

            // Manually manage the association between Client and Product without EF relationships
            var clientProduct = new ClientProduct
            {
                ClientId = clientId,
                ProductId = productId
            };

            // Add the client-product mapping
            _clientDbContext.ClientProducts.Add(clientProduct);
            await _clientDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}