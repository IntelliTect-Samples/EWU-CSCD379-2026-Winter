using Microsoft.AspNetCore.Mvc;
using BakeryApi.Models;
using BakeryApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace BakeryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            var created = await _productService.AddAsync(product);
            return Ok(created);
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            var imageUrl = await _productService
                .UploadImageAsync(file, HttpContext.Request);

            if (imageUrl == null)
                return BadRequest("No file uploaded.");

            return Ok(new { imageUrl });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
